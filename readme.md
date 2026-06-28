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

### DLL Management

Each DLL has an entry in the table with the basic name like `ToSic.Sxc.Dnn.Core`.
The main purpose is use it to build the list of DLLs, and to possibly sign all of it as not-exportable (IgnoreAll).
The information if a dll is all ok is processed the first time the DLL is analyzed and then cached to memory, so only some DLLs show a 

## History

### 2026-06-28 (@iJungleboy)

1. regenerate data models using v21 convention
1. introduce `DllGroup` for better UI; assign to all Dll data entries
1. move csharp into subfolder `Analyze` to keep things cleaner
1. introduce some docs
1. improve view (make wider)
1. group by DllGroup
