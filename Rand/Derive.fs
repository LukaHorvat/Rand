namespace Rand

open Microsoft.FSharp.Reflection
open System.Reflection 
open System
open System.Collections.Generic

module Derive =
    let toAGen gen : 'a Gen = Gen.map (fun (o : obj) -> o :?> 'a) gen
    let toObjGen gen : obj Gen = Gen.map (fun a -> a :> obj) gen 
    let dict = Dictionary() : Dictionary<Type, obj Gen>
    Seq.iter dict.Add [
        typedefof<int>, NativeGen.int |> toObjGen
        typedefof<double>, NativeGen.double |> toObjGen
        typedefof<string>, NativeGen.string |> toObjGen
        typedefof<bool>, NativeGen.bool |> toObjGen
        typedefof<char>, NativeGen.char |> toObjGen
    ]
    let inline genOfNative (t : Type) : obj Gen = dict.[t]
    let genOfArbitrary (t : Type) : obj Gen =
        let met = t.GetMethod("arbitrary", BindingFlags.Static ||| BindingFlags.Public)
        let pars = met.GetParameters()
        let genT = typedefof<Gen<_>>.MakeGenericType(t)
        let err s = failwith ("The static method 'arbitrary' of the type " + t.ToString() + " " + s)
        if pars.Length <> 1 then 
            err ("takes " + pars.Length.ToString() + " parameters, but is expected to take 1.")
        if pars.[0].ParameterType <> t then
            err ("takes a parameter of a type different than " + t.ToString() + ".")
        if met.ReturnType <> genT then
            err ("returns an object of the type " + met.ReturnType.ToString() + ", but is expected to return " + genT.ToString())
        let gen = met.Invoke(null, [|null|])
        let mapMethod = gen.GetType().GetMethod("Map")
        mapMethod.Invoke(gen, [||]) :?> obj Gen
    let rec fromProps (fields : PropertyInfo []) : obj [] Gen =
        Array.map (fun (prop : PropertyInfo) -> prop.PropertyType) fields
        |> Array.map genObj
        |> List.ofArray
        |> Gen.sequence
        |> Gen.map Array.ofList
    and genOfUnion (t : Type) : obj Gen = 
        let cases = FSharpType.GetUnionCases(t, false)
        gen {
            let! n = Gen.intTo (Array.length cases - 1) 
            let case = cases.[n]
            let argsGen = case.GetFields() |> fromProps
            return! Gen.map (fun args -> FSharpValue.MakeUnion(case, args, false)) argsGen
        }
    and genOfRecord (t : Type) : obj Gen =
        let fields = FSharpType.GetRecordFields(t, false)
        fromProps fields
        |> Gen.map (fun objs -> FSharpValue.MakeRecord(t, objs, false)) 
    and genObj (t : Type) : obj Gen =
        if dict.ContainsKey t then genOfNative t
        else if (t.GetMethod("arbitrary", BindingFlags.Static ||| BindingFlags.Public) <> null) then genOfArbitrary t
        else if (FSharpType.IsUnion(t, false)) then genOfUnion t
        else if (FSharpType.IsRecord(t, false)) then genOfRecord t
        else failwith ("Cannot derive a generator for the type " + t.ToString())
    let gen() : 'a Gen =
        let t = typeof<'a>
        genObj t |> toAGen