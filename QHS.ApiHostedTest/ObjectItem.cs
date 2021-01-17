using System;

namespace QHS.ApiHostedTest
{
    public class ObjectItem : IObjectItem
    {
        public void Escrever()
        {
            Console.WriteLine($"Chamando ObjectItem - {(DateTime.Now)}");
        }

        public void Escrever2()
        {
            Console.WriteLine($"Chamando ObjectItem method Escrever2 - {(DateTime.Now)}");
        }
    }
}
