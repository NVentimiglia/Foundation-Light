


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Foundation.Architecture.Tests
{
    [TestClass]
    public class ObservableTests
    {
        public class ViewModel : ObservableObject
        {
            private string _myProperty2;
            public string MyProperty2
            {
                get { return _myProperty2; }
                set
                {
                    if (_myProperty2 == value)
                        return;
                    _myProperty2 = value;
                    RaiseChange("MyProperty2", value);
                }
            }

            private int _myProperty;
            public int MyProperty
            {
                get { return _myProperty; }
                set
                {
                    if (_myProperty == value)
                        return;
                    _myProperty = value;
                    RaiseChange("MyProperty", value);
                }
            }

            public void MyCommand()
            {
                MyProperty++;
            }

            public void MyCommand2(int value)
            {
                MyProperty = (value + _myProperty);
            }

        }

        [TestMethod]
        public void TestObservableObject()
        {
            var vm = new ViewModel();
            int counter = 0;

            vm.OnPublish += (name) =>
            {
                Assert.AreEqual(name.Name, "MyProperty");
                Assert.AreEqual(name.Sender, vm);
                Assert.AreEqual(name.Value, vm.MyProperty);
                counter++;
            };

            vm.MyCommand();
            Assert.AreEqual(vm.MyProperty, 1);

            vm.MyCommand2(2);
            Assert.AreEqual(vm.MyProperty, 3);

            vm.MyProperty = 0;
            Assert.AreEqual(vm.MyProperty, 0);

            Assert.AreEqual(counter, 3);
        }


        [TestMethod]
        public void TestObservableProxy()
        {
            var vm = new ViewModel();
            var proxy = new ObservableProxy(vm);
            int counter2 = 0;
            int counter1 = 0;

            vm.OnPublish += (name) =>
            {
                counter1++;
            };

            proxy.OnPublish += (name) =>
            {
                counter2++;
            };

            proxy.Post("MyCommand");
            Assert.AreEqual(vm.MyProperty, 1);

            proxy.Post("MyCommand2", 2);
            Assert.AreEqual(vm.MyProperty, 3);

            proxy.Post("MyProperty", 0);
            Assert.AreEqual(vm.MyProperty, 0);

            Assert.AreEqual(counter1, 3);
            Assert.AreEqual(counter2, 3);

            proxy.Dispose();
        }


        [TestMethod]
        public void TestConversion()
        {
            var vm = new ViewModel();
            var proxy = new ObservableProxy(vm);
            int counter2 = 0;
            int counter1 = 0;

            vm.OnPublish += (name) =>
            {
                counter1++;
            };

            proxy.OnPublish += (name) =>
            {
                counter2++;
            };

            proxy.Post("MyCommand");
            Assert.AreEqual(vm.MyProperty, 1);

            proxy.Post("MyCommand2", "2");
            Assert.AreEqual(vm.MyProperty, 3);

            proxy.Post("MyProperty", 0f);
            Assert.AreEqual(vm.MyProperty, 0);

            Assert.AreEqual(counter1, 3);
            Assert.AreEqual(counter2, 3);


            proxy.Post("MyProperty2", "1");
            var temp = proxy.Get<int>("MyProperty2");
            Assert.AreEqual(temp, 1);

            proxy.Dispose();
        }

        [TestMethod]
        public void TestObservable()
        {
            var vm = new ObservableProperty<int>();

            vm.Value = 10;

            int counter = 0;

            vm.OnPublish += (v) =>
            {
                if(counter == 0)
                    Assert.AreEqual(10, v);
               counter++;
            };

            vm.Value = 0;

            vm.Value++;
            Assert.AreEqual(vm.Value, 1);

            vm.Value--;
            Assert.AreEqual(vm.Value, 0);

            vm.Value++;
            Assert.AreEqual(vm.Value, 1);

            Assert.AreEqual(counter, 5);
        }


        [TestMethod]
        public void TestList()
        {
            var vm = new ObservableList<string>();
            var v = new List<string>();

            Action test = () =>
            {
                Assert.IsTrue(v.Count == vm.Count);
                for (int i = 0; i < v.Count; i++)
                {
                    Assert.IsTrue(v[i] == vm[i]);
                }
            };

            vm.OnPublish += (args) =>
            {
                switch (args.Event)
                {
                    case ListChangedEventType.Add:
                        {
                            foreach (var item in args.Items)
                            {
                                v.Add(item as string);
                            }
                        }
                        break;
                    case ListChangedEventType.Remove:
                        {
                            foreach (var item in args.Items)
                            {
                                v.Remove(item as string);
                            }
                        }
                        break;
                    case ListChangedEventType.Replace:
                        {
                            var index = v.IndexOf(args.Items.ElementAt(0) as string);
                            if (index >= 0)
                            {
                                v[index] = args.Items.ElementAt(0) as string;
                            }
                        }
                        break;
                    case ListChangedEventType.Insert:
                        {
                            for (int j = 0; j < args.Items.Count(); j++)
                            {
                                v.Insert(j + args.Index, args.Items.ElementAt(j) as string);
                            }
                        }
                        break;
                    case ListChangedEventType.Clear:
                        {
                            v.Clear();
                        }
                        break;
                    case ListChangedEventType.Refresh:
                        {
                            //Nothing, flicker UI
                        }
                        break;
                }

                test();
            };


            vm.Add("0");

            for (int i = 1; i < 100; i++)
            {
                vm.Add(i.ToString());
            }


            for (int i = 1; i < 10; i++)
            {
                vm.Remove((i + 10).ToString());
            }

            vm.AddRange(new[] { "11", "12", "13" });

            vm.RemoveRange(new[] { "11", "12", "13" });

            vm.Replace("99");

            vm.Insert(3, "199");

            var h = vm.IndexOf("21");
            vm[h] = "21";

            vm.Clear();

        }

        [TestMethod]
        public void TestObservableMetrics()
        {
            var vm = new ViewModel();
            var proxy = new ObservableProxy(vm);
            var watch = new Stopwatch();
            long testNormal = long.MinValue;
            long testReflected = long.MinValue;
            long testProxy = long.MinValue;
            long testInvoke = long.MinValue;

            var invoke = (Action)Delegate.CreateDelegate(typeof(Action), vm, "MyCommand");

            //Dry run first before real test
            for (int k = 0; k < 5; k++)
            {
                watch.Reset();
                watch.Start();
                for (int i = 0; i < 1000; i++)
                {
                    vm.MyCommand();
                }
                watch.Stop();
                testNormal = watch.ElapsedTicks;

                watch.Reset();
                watch.Start();
                for (int i = 0; i < 1000; i++)
                {
                    proxy.Post("MyCommand");
                }
                watch.Stop();
                testProxy = watch.ElapsedTicks;


                watch.Reset();
                watch.Start();
                for (int i = 0; i < 1000; i++)
                {
                    invoke();
                }
                watch.Stop();
                testInvoke = watch.ElapsedTicks;

                watch.Reset();
                watch.Start();
                for (int i = 0; i < 1000; i++)
                {
                    vm.GetType().GetMethod("MyCommand").Invoke(vm, null);
                }
                watch.Stop();
                testReflected = watch.ElapsedTicks;
            }

            //Normal = 81
            //Reflected = 1088
            //Cached = 82
            //Proxy = 507
            Console.WriteLine("Normal = " + testNormal);
            Console.WriteLine("Reflected = " + testReflected);
            Console.WriteLine("Cached = " + testInvoke);
            Console.WriteLine("Proxy = " + testProxy);

        }
    }
}
