# Object Events

A object-specific message broker for relaying events routed by the message type. This is a drop-in replacement for GameObject.SendMessage

## Use

`````
        public void TestObjectRoute()
        {
            //define our message, a class
            var msg = new Msg { Content = MagicString };

            //define our routes (strings or game objects or something else)
            var route = new GameObject();

            //subscribe using the object reference
            ObjectEvents<GameObject, Msg>.Subscribe(route, Handler);

            //many ways to send
            ObjectEvents<GameObject, Msg>.Publish(route, msg);
            ObjectEvents.Publish(route, msg);
            ObjectEvents.Publish(route, msg, typeof(GameObject), typeof(Msg));

            //be sure to clean up as we are not using weak references
            ObjectEvents<GameObject, Msg>.Unsubscribe(route, Handler);
        }


        public void Handler(Msg myMessage)
        {
          //Do Something
        }
`````
