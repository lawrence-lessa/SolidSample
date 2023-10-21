using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace ArdalisRating
{
    /// <summary>
    /// The RatingEngine reads the policy application details from a file and produces a numeric 
    /// rating value based on the details.
    /// </summary>
    public class RatingEngine
    {
        public decimal Rating { get; set; }
        public ConsoleLogger Logger { get; set; } = new ConsoleLogger();
        public FilePolicySource PolicySource { get; set; } = new FilePolicySource();
        public JsonPolicySerializer PolicySerializer { get; set; } = new JsonPolicySerializer();
        public void Rate()
        {
            //Console.WriteLine("Starting rate.");
            Logger.Log("Starting rate.");

            //Console.WriteLine("Loading policy.");
            Logger.Log("Loading policy.");

            // load policy - open file policy.json
            //string policyJson = File.ReadAllText("policy.json");
            string policyJson = PolicySource.GetPolicyFromSource();

            //var policy = JsonConvert.DeserializeObject<Policy>(policyJson,
            //    new StringEnumConverter());
            var policy = PolicySerializer.GetPolicyFromJsonString(policyJson);

            switch (policy.Type)
            {
                case PolicyType.Auto:
                    //Console.WriteLine("Rating AUTO policy...");
                    Logger.Log("Rating AUTO policy...");
                    //Console.WriteLine("Validating policy.");
                    Logger.Log("Validating policy.");
                    if (String.IsNullOrEmpty(policy.Make))
                    {
                        //Console.WriteLine("Auto policy must specify Make");
                        Logger.Log("Auto policy must specify Make");
                        return;
                    }
                    if (policy.Make == "BMW")
                    {
                        if (policy.Deductible < 500)
                        {
                            Rating = 1000m;
                        }
                        Rating = 900m;
                    }
                    break;

                case PolicyType.Land:
                    //Console.WriteLine("Rating LAND policy...");
                    Logger.Log("Rating LAND policy...");
                    //Console.WriteLine("Validating policy.");
                    Logger.Log("Validating policy.");
                    if (policy.BondAmount == 0 || policy.Valuation == 0)
                    {
                        //Console.WriteLine("Land policy must specify Bond Amount and Valuation.");
                        Logger.Log("Land policy must specify Bond Amount and Valuation.");
                        return;
                    }
                    if (policy.BondAmount < 0.8m * policy.Valuation)
                    {
                        //Console.WriteLine("Insufficient bond amount.");
                        Logger.Log("Insufficient bond amount.");
                        return;
                    }
                    Rating = policy.BondAmount * 0.05m;
                    break;

                case PolicyType.Life:
                    //Console.WriteLine("Rating LIFE policy...");
                    Logger.Log("Rating LIFE policy...");
                    //Console.WriteLine("Validating policy.");
                    Logger.Log("Validating policy.");
                    if (policy.DateOfBirth == DateTime.MinValue)
                    {
                        //Console.WriteLine("Life policy must include Date of Birth.");
                        Logger.Log("Life policy must include Date of Birth.");
                        return;
                    }
                    if (policy.DateOfBirth < DateTime.Today.AddYears(-100))
                    {
                        //Console.WriteLine("Centenarians are not eligible for coverage.");
                        Logger.Log("Centenarians are not eligible for coverage.");
                        return;
                    }
                    if (policy.Amount == 0)
                    {
                        //Console.WriteLine("Life policy must include an Amount.");
                        Logger.Log("Life policy must include an Amount.");
                        return;
                    }
                    int age = DateTime.Today.Year - policy.DateOfBirth.Year;
                    if (policy.DateOfBirth.Month == DateTime.Today.Month &&
                        DateTime.Today.Day < policy.DateOfBirth.Day ||
                        DateTime.Today.Month < policy.DateOfBirth.Month)
                    {
                        age--;
                    }
                    decimal baseRate = policy.Amount * age / 200;
                    if (policy.IsSmoker)
                    {
                        Rating = baseRate * 2;
                        break;
                    }
                    Rating = baseRate;
                    break;

                default:
                    //Console.WriteLine("Unknown policy type");
                    Logger.Log("Unknown policy type");
                    break;
            }

            //Console.WriteLine("Rating completed.");
            Logger.Log("Rating completed.");
        }
    }
}
