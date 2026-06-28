# Models

All models should start with `Api` to separate them from similarly named things in System.Reflection.
The reason is that we want to be able to use the same names as in System.Reflection, but we also want to be able to use them in Razor without having to use `@using` for each of them.
