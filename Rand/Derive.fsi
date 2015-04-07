namespace Rand

module Derive =
    /// <summary>
    /// Constructs a generator that produces the given union type.
    /// </summary>
    val gen : unit -> 'a Gen