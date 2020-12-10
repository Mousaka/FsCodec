﻿module FsCodec.NewtonsoftJson.PicklerTests

open FsCodec.NewtonsoftJson
open Newtonsoft.Json
open Swensen.Unquote
open System
open Xunit

// NB Feel free to ignore this opinion and copy the 4 lines into your own globals - the pinning test will remain here
/// <summary>
///     Renders all Guids without dashes.
/// </summary>
/// <remarks>
///     Can work correctly as a global converter, as some codebases do for historical reasons
///     Could arguably be usable as base class for various converters, including the above.
///     However, the above pattern and variants thereof are recommended for new types.
///     In general, the philosophy is that, beyond the Pickler base types, an identiy type should consist of explicit
///       code as much as possible, and global converters really have to earn their keep - magic starts with -100 points.
/// </remarks>
type GuidConverter() =
    inherit JsonIsomorphism<Guid, string>()
    override __.Pickle g = g.ToString "N"
    override __.UnPickle g = Guid.Parse g

type WithEmbeddedGuid = { a: string; [<JsonConverter(typeof<GuidConverter>)>] b: Guid }

let [<Fact>] ``Tagging with GuidConverter`` () =
    let value = { a = "testing"; b = Guid.Empty }

    let result = JsonConvert.SerializeObject value

    test <@ """{"a":"testing","b":"00000000000000000000000000000000"}""" = result @>

let [<Fact>] ``Global GuidConverter`` () =
    let value = Guid.Empty

    let resDashes = JsonConvert.SerializeObject(value, Settings.Create())
    let resNoDashes = JsonConvert.SerializeObject(value, Settings.Create(GuidConverter()))

    test <@ "\"00000000-0000-0000-0000-000000000000\"" = resDashes
            && "\"00000000000000000000000000000000\"" = resNoDashes @>
