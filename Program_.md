using System.Xml.Serialization;
using Microsoft.SolverFoundation.Services; 

namespace Solver {
  
    public class Program  {  
        static void Main(string[] args)  {  
        SolverContext context = SolverContext.GetContext();  
        Model model = context.CreateModel();  
  
        Decision food1 = new Decision(Domain.RealNonnegative, "food1");  
        Decision food2 = new Decision(Domain.RealNonnegative, "food2");  
        model.AddDecisions(food1, food2);  
  
        // Add constraints  
        model.AddConstraints("limits",  
            0 <= food1 <= 10,  
            0 <= food2 <= 20  
        );  
  
        // Set goal  
        model.AddGoal("cost", GoalKind.Minimize, 10 * food1 + 20 * food2);  
  
        // Solve  
        Solution solution = context.Solve();  
  
        // Display results  
        Console.WriteLine("food1: {0}", food1.GetDouble());  
        Console.WriteLine("food2: {0}", food2.GetDouble());  
        
        }  
    }  
}

