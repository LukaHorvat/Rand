namespace Rand

[<Sealed>]
type 'a Gen =
    member Map : unit -> obj Gen

[<Class>]
[<Sealed>]
type GenBuilder =
    member Bind : 'a Gen * ('a -> 'b Gen) -> 'b Gen
    member Return : 'a -> 'a Gen
    member ReturnFrom : 'a Gen -> 'a Gen

[<AutoOpen>]
module GenBuild =
    val gen : GenBuilder

module GenOps =
    /// <summary>
    /// Operator version of the map function.
    /// </summary>
    /// <param name="f">function to map</param>
    /// <param name="g">generator to map over</param>
    val (<!>) : ('a -> 'b) -> 'a Gen -> 'b Gen
    /// <summary>
    /// Operator version of the ap function.
    /// </summary>
    /// <param name="gf">function generator</param>
    /// <param name="ga">value generator</param>
    val (<*>) : ('a -> 'b) Gen -> 'a Gen -> 'b Gen
    /// <summary>
    /// Operator version of the bind function.
    /// </summary>
    /// <param name="g">generator to bind</param>
    /// <param name="f">binding function</param>
    val (>>=) : 'a Gen -> ('a -> 'b Gen) -> 'b Gen
    /// <summary>
    /// Operator version of the alternative function.
    /// </summary>
    /// <param name="g1">the first generator</param>
    /// <param name="g2">the second generator</param>
    val (<|>) : 'a Gen -> 'a Gen -> 'a Gen
    

module Gen =
    open System

    /// <summary>
    /// Operator version of the map function.
    /// </summary>
    /// <param name="f">function to map</param>
    /// <param name="g">generator to map over</param>
    val (<!>) : ('a -> 'b) -> 'a Gen -> 'b Gen
    /// <summary>
    /// Operator version of the ap function.
    /// </summary>
    /// <param name="gf">function generator</param>
    /// <param name="ga">value generator</param>
    val (<*>) : ('a -> 'b) Gen -> 'a Gen -> 'b Gen
    /// <summary>
    /// Operator version of the bind function.
    /// </summary>
    /// <param name="g">generator to bind</param>
    /// <param name="f">binding function</param>
    val (>>=) : 'a Gen -> ('a -> 'b Gen) -> 'b Gen
    /// <summary>
    /// Operator version of the alternative function.
    /// </summary>
    /// <param name="g1">the first generator</param>
    /// <param name="g2">the second generator</param>
    val (<|>) : 'a Gen -> 'a Gen -> 'a Gen
    
    /// <summary>
    /// Constructs a new generator out of a function that produces values
    /// based on the current random generator.
    /// </summary>
    /// <param name="f">the generating function</param>
    val fromGenerator : (ImmutableRandom -> 'a) -> 'a Gen
    /// <summary>
    /// Constructs a new generator that always produces the same value.
    /// </summary>
    /// <param name="a">the value to produce</param>
    val constant : 'a -> 'a Gen
    /// <summary>
    /// Constructs a new generator from a generator and a function that
    /// can operate on it's product and provide a new generator.
    /// This lets you create a generator that depends on the values
    /// produced by the previous one.
    /// </summary>
    /// <param name="g">generator to bind</param>
    /// <param name="f">binding function</param>
    val bind : 'a Gen -> ('a -> 'b Gen) -> 'b Gen
    /// <summary>
    /// Using an initial random generator, runs the generator to produce
    /// a value.
    /// </summary>
    /// <param name="g">generator</param>
    /// <param name="rand">initial random generator</param>
    val runGen : 'a Gen -> ImmutableRandom -> 'a
    /// <summary>
    /// Maps a function over the product of the generator.
    /// </summary>
    /// <param name="f">function to map</param>
    /// <param name="g">generator to map over</param>
    val map : ('a -> 'b) -> 'a Gen -> 'b Gen
    /// <summary>
    /// Applies the generated function from the first generator to the
    /// product of the second generator yielding a new generator.
    /// </summary>
    /// <param name="gf">function generator</param>
    /// <param name="ga">value generator</param>
    val ap : ('a -> 'b) Gen -> 'a Gen -> 'b Gen
    /// <summary>
    /// Takes a generator that produces a generator that produces a value
    /// and combines the nested generators into a single one that produces
    /// a value.
    /// </summary>
    /// <param name="gga">nested generators</param>
    val join : 'a Gen Gen -> 'a Gen
    /// <summary>
    /// Takes a list of generator of value and constructs a generator that
    /// produces a list of values. The new generator produces a list by
    /// generating the first element with the first generator in the original
    /// list, the second element with the second generator and so on.
    /// </summary>
    /// <param name="gs">list of generators</param>
    val sequence : 'a Gen list -> 'a list Gen
    /// <summary>
    /// Constructs a generator that produces an int from 0 to n inclusive.
    /// </summary>
    /// <param name="n">maximum value</param>
    val intTo : int -> int Gen
    val oneOf : 'a Gen list -> 'a Gen
    /// <summary>
    /// Constructs a generator that produces one of the values from
    /// a list with equal probability.
    /// </summary>
    /// <param name="xs">possible values</param>
    val elementOf : 'a list -> 'a Gen
    /// <summary>
    /// Constructs a generator that produces a list of values using a single
    /// generator.
    /// </summary>
    /// <param name="g">element generator</param>
    /// <param name="n">number of elements to generate</param>
    val listOf : 'a Gen -> int -> 'a list Gen
    /// <summary>
    /// Constructs a generator the produces a value via either the
    /// first generator or the second one.
    /// </summary>
    /// <param name="g1">the first generator</param>
    /// <param name="g2">the second generator</param>
    val alternative : 'a Gen -> 'a Gen -> 'a Gen