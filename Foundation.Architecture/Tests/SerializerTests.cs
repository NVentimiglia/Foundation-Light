using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Foundation.Architecture.Tests
{
    [TestClass]
    public class JSONSerializerTests
    {
        #region Models

        public class BigObject
        {
            //1024 * 100
            public string BigString { get; set; }
            //1024 * 4
            public string SmallString { get; set; }
        }

        public enum MyEnum : byte
        {
            E_A,
            A_B,
            E_C
        }

        public struct Vector
        {
            public float x;
            public float y;

        }

        public class ComplexObject
        {
            //10

            public int[] a { get; set; }
            public ushort[] b { get; set; }
            public MyEnum[] c { get; set; }
            public Vector[] d { get; set; }
            public float[] e { get; set; }
        }

        public class Animal
        {
            public string A { get; set; }

            public Animal()
            {
                A = "ANIMAL";
            }
        }

        public class Cat : Animal
        {
            public string S { get; set; }

            public Cat()
            {
                A = S = "CAT";
            }
        }

        public class Dog : Animal
        {
            public string S { get; set; }

            public Dog()
            {
                A = S = "Dog";
            }
        }

        public class InheritenceObject
        {
            // Animal, Cat, Dog, Cat, Dog, Animal
            public List<Animal> Animals { get; set; }
        }

        // Bonus, does this work with any socket technology
        public class MockSocket
        {
            Stack<byte[]> _buffers = new Stack<byte[]>();

            public void Write(byte[] buffer)
            {
                _buffers.Push(buffer);
            }

            public bool CanRead()
            {
                return _buffers.Count > 0;
            }

            public byte[] Read()
            {
                return _buffers.Pop();
            }
        }
        #endregion

        public static string GetString(int length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0;i < length;i++)
            {
                sb.Append("A");
            }
            return sb.ToString();
        }

        static long mem;
        static long vmem;
        static long threads;

        public static void StartMem()
        {
            GC.Collect();

            using (var proc = Process.GetCurrentProcess())
            {
                mem = proc.PrivateMemorySize64 / (1024 * 1024);
                vmem = proc.VirtualMemorySize64 / (1024 * 1024);
                threads = proc.Threads.Count;
            }
        }
        public static void EndMem()
        {

            using (var proc = Process.GetCurrentProcess())
            {
                mem =  proc.PrivateMemorySize64 / (1024 * 1024) - mem;
                vmem = proc.VirtualMemorySize64 / (1024 * 1024) - vmem;
                threads =  proc.Threads.Count - threads;
            }

            Console.WriteLine("MEM " + mem + " VMEM " + vmem + " THREADS " + threads);
            GC.Collect();
        }

        public static void AssertArray<T>(T[] a, T[] b)
        {
            for (int j = 0;j < a.Length;j++)
            {
                Assert.AreEqual(a[j], b[j]);
            }
        }

        [TestMethod]
        public void BigObjectTest()
        {
            StartMem();

            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0;i < 1000;i++)
            {
                var model = new BigObject
                {
                    BigString = GetString(1024 * 100),
                    SmallString = GetString(1024 * 4)
                };

                var json = JsonConvert.SerializeObject(model);
                var copy = JsonConvert.DeserializeObject<BigObject>(json);
                Assert.AreEqual(model.BigString, copy.BigString);
                Assert.AreEqual(model.SmallString, copy.SmallString);
            }
            watch.Stop();
            Console.WriteLine("BigObjectTest.Json = " + watch.ElapsedMilliseconds + " ms");
            EndMem();
        }

        [TestMethod]
        public void ComplexObjectTest()
        {
            StartMem();
            
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0;i < 1000;i++)
            {
                var model = new ComplexObject()
                {
                    a = new[]
                {
                    1,2,3,4,5,6,7,8,9,0
                },
                    b = new ushort[]
                {
                    1,2,3,4,5,6,7,8,9,0
                },
                    c = new[]
                {
                   MyEnum.A_B, MyEnum.E_A,MyEnum.E_C
                },
                    d = new[]
                {
                    new Vector { x = 0, y = 0},
                    new Vector { x = 1, y = 1},
                    new Vector { x = 2, y = 2},
                    new Vector { x = 3, y = 3},
                }
                };

                var json = JsonConvert.SerializeObject(model);
                var copy = JsonConvert.DeserializeObject<ComplexObject>(json);
                AssertArray(model.a, copy.a);
                AssertArray(model.b, copy.b);
                AssertArray(model.c, copy.c);
                AssertArray(model.d, copy.d);
            }
            watch.Stop();
            Console.WriteLine("ComplexObjectTest.Json = " + watch.ElapsedMilliseconds + " ms");
            EndMem();
        }

        // Json Failed
        //[TestMethod]
        //public void InheritenceObjectTest()
        //{
        //    var model = new InheritenceObject()
        //    {
        //        Animals = new List<Animal>()
        //        {
        //            new Animal(),
        //            new Cat(),
        //            new Dog(),
        //        }
        //    };

        //    Stopwatch watch = new Stopwatch();
        //    watch.Start();
        //    for (int i = 0;i < 1000;i++)
        //    {

        //        var json = JsonConvert.SerializeObject(model);
        //        var copy = JsonConvert.DeserializeObject<InheritenceObject>(json);

        //        Assert.AreEqual(model.Animals[0].A, copy.Animals[0].A);
        //        Assert.AreEqual(model.Animals[1].A, copy.Animals[1].A);
        //        Assert.AreEqual(model.Animals[2].A, copy.Animals[2].A);

        //        Assert.AreEqual(model.Animals[0].GetType(), copy.Animals[0].GetType());
        //        Assert.AreEqual(model.Animals[1].GetType(), copy.Animals[1].GetType());
        //        Assert.AreEqual(model.Animals[2].GetType(), copy.Animals[2].GetType());
        //    }
        //    watch.Stop();
        //    Console.WriteLine("InheritenceObjectTest.Json = " + watch.ElapsedMilliseconds + " ms");
        //    LogMem();
        //}
    }
}