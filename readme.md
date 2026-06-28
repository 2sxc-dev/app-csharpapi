# 2sxc CSharp API Analyzer App

> **Purpose of this Tool**
>
> This tool is meant to review all APIs which 2sxc has to ensure that a) we control what is public (at two levels) and b) we know if we have documentation for it.
> This is to ensure that generated XML files (for VS-Code etc.) will be useful and not bleed out internal stuff, and that most hidden APIs also don't appear in intellisense.

These are just notes to future me, so I can remember how this works. This is not meant for end users, but for developers who want to understand how this works.

## How it works

Basically it's a tree of rules to determine what should happen in a specific area,
and using this the UI should if something isn't as should be.
This is then refined by either fixing the code or by adding a rule to again specify that area should behave as it does.
The rules are stored in 2sxc.
Rules are about:

- Which DLLs should be considered `Dll`
- Which Namespaces should be considered `RuleNamespace`
- Which Types should be considered `RuleClass`
- Which Members should be considered `Rule???`

## How we Work with it

1. First select a DLL to analyze. This will show all namespaces and types in that DLL.
1. Many namespaces should be ok (showing a ✅)
1. If some are not ok, we must determine what to do with it
    1. 
1. Then select a namespace to analyze. This will show all types in that namespace.

What could be not-ok?

1. Often we have a public API which is either missing the `[PublicApi]` attribute or has a `[PrivateApi]` attribute.
    This is a problem because it will be exported to the XML file and thus show up in intellisense, even though we don't want it to.
1. ...and/or it's missing the `[ShowApiWhenReleased(ShowApiMode.Never)]` attribute
1. or it has conflicting information (e.g. `[PublicApi]` and `[ShowApiWhenReleased(ShowApiMode.Never)]`)

In such situations, we should either sync the attributes, or add an explicit rule that this is how we want it to be.

Typical fixes are:

1. make something internal (non-public) to avoid the API from being exported
1. Add `[PublicApi]` to make it public and thus exported; or `[PrivateApi]` to make it private and thus not exported
1. Add `[ShowApiWhenReleased(ShowApiMode.Never)]` to make it not exported (we don't usually add `[ShowApiWhenReleased(ShowApiMode.Always)]` as that's the default)
1. Add a rule to skip checking this (🦘) - typically for some system namespaces like `System.Diagnostics` which we don't care about so we shouldn't see warnings.

### DLL Management

Each DLL has an entry in the table with the basic name like `ToSic.Sxc.Dnn.Core`.
The main purpose is use it to build the list of DLLs, and to possibly sign all of it as not-exportable (IgnoreAll).
The information if a dll is all ok is processed the first time the DLL is analyzed and then cached to memory, so only some DLLs show a 

## To Do

1. improve explicit that `[InternalApi_DoNotUse_MayChangeWithoutNotice]` works with show-never and doesn't create warning
1. Add ability to scan all DLLs and put into cache

## History

### 2026-06-28 (@iJungleboy)

1. regenerate data models using v21 convention
1. introduce `DllGroup` for better UI; assign to all Dll data entries
1. move csharp into subfolder `Analyze` to keep things cleaner
1. introduce some docs
1. improve view (make wider)
1. group by DllGroup
1. Fix detection of `ShowApiWhenReleased` during debug build, which replaces it with another class
1. Change loading Assembly info to always recreate for the current DLL
1. Separate AssemblyInfo object from the service which creates it
1. Separate TypeInfo object from the service which creates it
