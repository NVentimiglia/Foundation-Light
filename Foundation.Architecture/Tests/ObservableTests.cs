using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Foundation.Architecture.Tests
{
    [TestClass]
    public class ObservableTests
    {
        public class ViewModel : ObservableObject
        {
            public Observable<int> MyObservable = new Observable<int>();

            private int _myProperty;
            public int MyProperty
            {
                get { return _myProperty; }
                set
                {
                    if (_myProperty == value)
                        return;
                    _myProperty = value;
                    RaisePropertyChanged("MyProperty");
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

            vm.OnPropertyChanged += (name) =>
            {
                Assert.AreEqual(name, "MyProperty");
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
        public void TestObservable()
        {
            var vm = new Observable<int>();
            int counter = 0;

            vm.OnValueChange += (v) =>
            {
                counter++;
            };

            vm.Value++;
            Assert.AreEqual(vm.Value, 1);

            vm.Value--;
            Assert.AreEqual(vm.Value, 0);

            vm.Value++;
            Assert.AreEqual(vm.Value, 1);

            Assert.AreEqual(counter, 3);
        }

        [TestMethod]
        public void TestObservableProxy()
        {
            var vm = new ViewModel();
            var proxy = new ObservableProxy(vm);
            int counter2 = 0;
            int counter1 = 0;

            vm.OnPropertyChanged += (name) =>
            {
                counter1++;
            };

            proxy.OnPropertyChanged += (name) =>
            {
                counter2++;
            };

            proxy.Invoke("MyCommand");
            Assert.AreEqual(vm.MyProperty, 1);

            proxy.Set("MyCommand2", 2);
            Assert.AreEqual(vm.MyProperty, 3);

            proxy.Set("MyProperty", 0);
            Assert.AreEqual(vm.MyProperty, 0);

            Assert.AreEqual(counter1, 3);
            Assert.AreEqual(counter2, 3);


            proxy.Set("MyObservable", 3);
            Assert.AreEqual(proxy.Get<int>("MyObservable"), 3);
            Assert.AreEqual(counter2, 4);

            proxy.Dispose();

            vm.MyObservable = 0;
            Assert.AreEqual(counter2, 4);

        }
    }
}
