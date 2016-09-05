#Injector

For the resolving of dependencies.

### RegisterTransient

A new instance for each get. Must register a factory or a type.

`````
    //Register a concrete type keyed by an interface
    Container.RegisterTransient<IWordService, WordService>();
    
    //Register a concrete type keyed by its own (concrete) type
    Container.RegisterTransient<SentanceService>();
        
    //Register a concrete type keyed by its own (concrete) type and a custom factory action
    Container.RegisterTransient<Document>(()=> new Document());
`````

### RegisterSingleton

A static instance.

`````
    //Register a concrete type keyed by an interface
    Container.RegisterSingleton<IWordService, WordService>();
    
    //Register a concrete type keyed by its own (concrete) type
    Container.RegisterSingleton<SentanceService>();
        
    //Register a concrete type keyed by its own (concrete) type and a custom factory action
    Container.RegisterSingleton<Document>(()=> new Document());
        
    //Register a concrete type keyed by its own (concrete) type and a pre instantiated instance
    //All loads are lazy, except this one.
    var document = new Document();
    Container.RegisterSingleton<Document>(document);
`````

### Get

get from the container.

`````
    Container.RegisterSingleton<IWordService, WordService>();
    Container.RegisterSingleton<SentanceService>();
    
    var words = Container.Get<IWordService>();
    var sentances = Container.Get<SentanceService>();
`````

### Inject Into

Uses reflection to resolve dependencies. Useful for daisy chaining.

`````
public class MyScript
{
    [Inject]
    protected IWordService Words;
    
    void Awake()
    {
        Container.RegisterSingleton<IWordService, WordService>();
        Container.InjectInto(this);
        Asset.IsNotNull(Words);
    }
}
`````


### Implementation Details (Gotchas)

- Since we are using a dictionary to key references internally, if you register with an interface you must request with that same interface. If you reference with the concrete type, you must get using that concrete type.
- All lazy loaded dependencies will pass to the InjectInto method when they generated. That means if they have dependencies, they should be injected for you.
- Please Register dependencies lowest level first, to minimize the risk of not finding things.