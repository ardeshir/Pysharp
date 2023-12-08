using System;
using System.Collections.Generic;
using Gurobi;

class Program {
    static void Main() {

        try
        {
            GRBEnv env = new GRBEnv();
            GRBModel model = new GRBModel(env);

            model.ModelName = "diet";
            // Nutrition guidelines
            var categories = new Dictionary<string, Tuple<double, double>>
            {
                ["calories"] = Tuple.Create(1800.0, 2200.0),
                ["protein"] = Tuple.Create(91.0, GRB.INFINITY),
                ["fat"] = Tuple.Create(0.0, 65.0),
                ["sodium"] = Tuple.Create(0.0, 1779.0)
            };
            // Foods and costs
            var foods = new Dictionary<string, double>
            {
                ["hamburger"] = 2.49,
                ["chicken"] = 2.89,
                // Add other foods here...
            };
            // Nutrition values
            var nutritionValues = new Dictionary<Tuple<string, string>, double>
            {
                [Tuple.Create("hamburger", "calories")] = 410,
                // Add other nutrition values here...
            };
            // Decision variables
            var buy = new Dictionary<string, GRBVar>();
            foreach (var food in foods.Keys)
            {
                buy[food] = model.AddVar(0, GRB.INFINITY, 0, GRB.CONTINUOUS, food);
            }
            // Objective: minimize cost
            GRBLinExpr obj = new GRBLinExpr();
            foreach (var food in foods.Keys)
            {
                obj.AddTerm(foods[food], buy[food]);
            }
            model.SetObjective(obj, GRB.MINIMIZE);
            // Nutrition constraints
            foreach (var category in categories.Keys)
            {
                GRBLinExpr lhs = new GRBLinExpr();
                foreach (var food in foods.Keys)
                {
                    var key = Tuple.Create(food, category);
                    if (nutritionValues.ContainsKey(key))
                    {
                        lhs.AddTerm(nutritionValues[key], buy[food]);
                    }
                }
                model.AddRange(lhs, categories[category].Item1, categories[category].Item2, category);
            }
          // Solve
            model.Optimize();
            PrintSolution(model, buy, foods.Keys);
            // Adding new constraint
            Console.WriteLine("\nAdding constraint: at most 6 servings of dairy");
            GRBLinExpr dairy = new GRBLinExpr();
            dairy.AddTerm(1.0, buy["milk"]);
            dairy.AddTerm(1.0, buy["ice cream"]);
            model.AddConstr(dairy <= 6, "limit_dairy");
            // Re-Solve
            model.Optimize();
            PrintSolution(model, buy, foods.Keys);
        }
        catch (GRBException e)
        {
            Console.WriteLine("Error code: " + e.ErrorCode + ". " + e.Message);
        }
    }

    static void PrintSolution(GRBModel model, Dictionary<string, GRBVar> buy, IEnumerable<string> foods)
    {
        if (model.Status == GRB.Status.OPTIMAL)
        {
            Console.WriteLine("\nCost: " + model.ObjVal);
            Console.WriteLine("\nBuy:");
            foreach (var food in foods)
            {
                if (buy[food].X > 0.0001)
                {
                    Console.WriteLine(food + " " + buy[food].X);
                }
            }
        }
        else
        {
            Console.WriteLine("No solution");
        }
    }
}