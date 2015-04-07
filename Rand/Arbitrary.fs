namespace Rand

open System
open GenOps

module NativeGen =
    let unit = Gen.constant ()
    let int = Gen.fromGenerator (fun rand -> ImmutableRandom.int rand)
    let double = Gen.fromGenerator (fun rand -> ImmutableRandom.double rand)
    let char = Gen.elementOf [(char)32 .. (char)126]
    let string = Gen.intTo 7 >>= Gen.listOf char |> Gen.map (Array.ofList >> String.Concat)
    let bool = Gen.elementOf [true; false]

module Arb =
    type public Default =
        | [<Core.CompilerMessage("Default should never be written manually", 0)>]Default
        static member arbitrary(_ : unit) = NativeGen.unit
        static member arbitrary(_ : int) = NativeGen.int

    let inline internal arbitraryInternal< ^a, ^b when (^a or ^b) : (static member arbitrary : ^a -> ^a Gen)> (b : ^b) : ^a Gen
        = ((^a or ^b) : (static member arbitrary : ^a -> ^a Gen) (Unchecked.defaultof< ^a>))
        
    /// <summary>
    /// Constructs a default generator that produces arbitrary data of the requested type.
    /// </summary>
    let inline public arbitrary () : ^a Gen = arbitraryInternal Default