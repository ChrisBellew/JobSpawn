using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JobSpawn.Common;
using Microsoft.Owin.Hosting;

namespace JobSpawn.ExampleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Thread.Sleep(1000);

            //string baseAddress = "http://localhost:9000/";

            //using (WebApp.Start<Startup>(url: baseAddress))
            //{
            var primeCalculator = Spawner.Spawn<PrimeCalculator>().As<IPrimeCalculator>();
            Console.WriteLine(primeCalculator.GetNumber());
            //Console.WriteLine(string.Join(", ", primeCalculator.FindPrimes(0, 10)));
            Console.WriteLine("Ran!");
            Console.ReadLine();
            //}

            /*var primeCalculators = Spawner.SpawnMany<PrimeCalculator>(5);
            var primes = primeCalculators.SelectMany((x, i) => x.FindPrimes(i * 10, i * 10 + 10));
            Console.WriteLine(string.Join(", ", primes));*/

            Console.ReadKey();
        }
    }

    public class PrimeCalculator : IPrimeCalculator
    {
        /*public IEnumerable<int> FindPrimes(int start, int end)
        {
            return Enumerable.Range(start, end - start).Where(IsPrime);
        }

        protected bool IsPrime(int number)
        {
            for (var i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number%i == 0)
                {
                    return false;
                }
            }

            return true;
        }*/

        public int GetNumber()
        {
            return 3;
        }
    }

    public interface IPrimeCalculator
    {
        //IEnumerable<int> FindPrimes(int start, int end);
        int GetNumber();
    }

    /*public class ProxyPrimeCalculator : PrimeCalculator
    {
        public new IEnumerable<int> FindPrimes(int start, int end)
        {
            return Enumerable.Range(start, end - start).Where(IsPrime);
        }
    } */

}
