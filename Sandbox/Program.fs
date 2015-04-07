// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open Rand
open System

type Color = Red | Green | Blue with
    static member arbitrary(_ : Color) = Gen.constant Red

type Test = {
    X : Color
    Y : Option<bool>
    }

[<EntryPoint>]
let main argv = 
    let test = Derive.gen<Test> ()
    printfn "%A" (ImmutableRandom.fromSeed 9 |> Gen.runGen (Gen.listOf test 20))
    Console.ReadKey()
    0 // return an integer exit code
