# `ImpossibleOdds.DataMapping`
The `ImpossibleOdds.DataMapping` module is probably the most abstract module and is not directly usable or useful. It functions as a backbone to many of the other modules:

* Json: it maps objects to/from the internal JSON object and array data structures.

* Http: it maps objects to an HTTP POST request body, and allows to map fields from request classes to be used as header and URL parameters.

* Photon Webhooks: transforms objects to a compatible data structure that can be used by the Photon library to send over the network.

As you can see, its main goal is to map data from one form to another. If you're interested in the inner workings of the data mapping module, or would like to create a custom mapping, read on! Otherwise, you can skip this module all together.

The datamapping process works by providing it some piece of data, and according to a given 'mapping definition', it pushes the data through a defined series of small 'processors' that either accept the data, or let it fall through to the next processor. Consider it a vastly more complex `switch` statement.

This module can be extended to build a custom mapping system if your project requires it. It is best to start with defining a custom mapping definition. A basic mapping definition at least consists of two sets of data processors and a set of supported types, i.e. primitive types, that are supported as is, such as `int`, `float` or `string`. Have a look at the `IMappingDefinition` interface.

```csharp
// TODO: write basic mapping definition example
```

This basic definition is used to further construct more restrictive interfaces that give more control on the kind of data that is processed. Simple implementations are also provided to relief from boiler-plate code:

* `IIndexBasedMappingDefinition` and `IndexBasedMappingDefinition`: define the custom attribute types that will get used to map your data structure to a list-like structure, i.e. an array or a list.

* `ILookupBasedMappingDefinition` and `LookupBasedMappingDefinition`: define the custom attribute types that will get used to map your data structure to a lookup-like structure, i.e. a dictionary.

* `DualMappingDefinition`: a class that implements both an index-based and lookup-based mapping definition described above.

Both of the interfaces above are backed with generic variants to further restrict and help prevent making errors in the design process.

Each mapping definition must also define two sets of processors:

* `IMapToDataStructureProcessor`: a processor that takes a piece of data and transforms it to a type as supported by the mapping definition.

* `IMapFromDataStructureProcessor`: a processor that takes a piece of data that is supported by the mapping definition and attempts to transfrom it to a given data type.

Most data processors implement both interfaces at the same time, as they show great similarity on how they deal with each direction of transforming the data. To support the other modules mentioned above, several such processors have already been defined:

* `ExactMatchMappingProcessor`: if the given data exactly matches a supported type of the mapping definition, nothing really needs to be transformed.

* `PrimitiveTypeMappingProcessor`: upcast a primitive value to another primitive from the set of supported types.

* `DateTimeMappingProcessor`: transforms a `DateTime` structure to/from a string of a defined format.

* `DecimalMappingProcessor`: transfroms a `decimal` structure to/from a string value.

* `EnumMappingProcessor`: transforms a enumeration value to/from the underlying type, or a string value, if defined.

* `StringMappingProcessor`: only supports transforming from a string value to a target type if supported by the `Convert` method.

* `SequenceMappingProcessor`: only usable if an `IIndexBasedMappingDefinition` is used. Transform one list-like structure to/from another. All elements will also get transfromed individually.

* `LookupMappingProcessor`: only usable if an `ILookupBasedMappingDefinition` is used. Transfrom one lookup-like structure to/from another. All keys and values will also get transformed individually.

* `CustomObjectSequenceMappingProcessor`: only usable if an `IIndexBasedMappingDefinition` is used. Transfroms a complex object to/from a defined list-like structure. The list-like structure is required to implement the `IEnumerable` interface, i.e. arrays and `List<T>`.

* `CustomObjectLookupMappingProcessor`: only usable if an `ILookupBasedMappingDefinition` is used. Transfroms a complex object to/from a defined lookup-like structure. The lookup-like structure is required to implement the `IDictionary` interface, i.e. `Dictionary<TKey,TValue>`.

In designing your custom mapping definition, consider the order in which you define the processors in each set. It may impact both performance and the ability to successfully process certain data structures. For example, consider a `Dictionary<,>` object, it also implements the `IEnumerable` interface. If you insert a `SequenceMappingProcessor` before a `LookupMappingProcessor`, the `SequenceMappingProcessor` consume the data and will attempt to process it. It will fail though, since the key-value pair data structure of the `Dictionary<,>` type is not directly transformable. So keep this in mind.

Have a look at the following classes as example implementations mapping definitions; `HttpBodyMappingDefinition`, `HttpHeaderMappingDefinition`, `HttpURLMappingDefinition`, `WebhookMappingDefinition` and `JsonMappingDefinition`.
