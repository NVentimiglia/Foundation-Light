# Domain Events

A global message broker for relaying events routed by the objects type. This pub sub service is perfect for decoupling logical consumers from infrastructure.

This implementation makes use of static generics. This alows us to not use reflection. On IOS and other older AOT platforms this can cause error of 'not enough trampolines', which can be fixed by config setting.

## Use

`````
            //Define a message class to send
            var msg = new Msg {Content = MagicString};
            
            //subscribe you handlers
            DomainEvents<Msg>.Subscribe(Handler);

            //maby ways to send
            DomainEvents<Msg>.Publish(msg);
            DomainEvents.Publish(msg);
            DomainEvents.Publish(msg, typeof(Msg));

            //unsubcribe when done as we are not using weakreferences internally
            DomainEvents<Msg>.Unsubscribe(Handler);
            
            
            public void Handler(Msg myMessage)
            {
              //Do Something
            }
`````
