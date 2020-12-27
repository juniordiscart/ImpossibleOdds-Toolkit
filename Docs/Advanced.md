# ![Impossible Odds Logo][Logo] Unity C# Toolkit - Advanced

The sub-sections found below are most likely not sections you'll come in direct contact with unless you wish to build and expand upon the features provided in this toolkit.

## Weblink

The `ImpossibleOdds.Weblink` namespace contains a general purpose messaging framework for sending requests and processing matching responses. It performs an automatic response class mapping in order to formulate the response data in a way which makes them directly workable for your classes.

> Documentation is under construction and API is not finalized!

It's currently used by the [`ImpossibleOdds.Http`][Http] namespace.

## Serialization

The `ImpossibleOdds.Serialization` namespace has several data processing classes to transform your data to/from intermediate data structures. This intermediate data can then be used to easily process them to other formats. What this intermediate data looks like can be defined using several flavors of 'serialization definitions'. They define what data types are natively supported by the target data format, and which data processors are used to transform the more complex data.

> Documentation is under construction and API is not finalized!

This is extensively used by the [`ImpossibleOdds.Json`][Json] and [`ImpossibleOdds.Http`][Http] namespaces.

[Logo]: ./Images/ImpossibleOddsLogo.png
[Json]: ./Json.md
[Http]: ./Http.md
