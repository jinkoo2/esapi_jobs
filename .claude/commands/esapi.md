ESAPI project management tool.

## Usage

```
/esapi create <project_type> '<project_name>'
/esapi build '<project_name>' <configuration>
/esapi run '<project_name>' <configuration>
```

## Arguments

The user will provide arguments after `/esapi` in the format shown above.

Parse `$ARGUMENTS` to determine which subcommand is being used (`create`, `build`, or `run`), then follow the corresponding instructions below.

---

## Subcommand: `create`

### Arguments

- `project_type`: One of `standalone`, `binary_plugin`, or `single_file_plugin`
- `project_name`: The name for the new project (use snake_case, e.g. `hello_world`)

### Directory Structure

```
<workspace_root>/
  esapi/
    templates/          # Template sources (do NOT modify)
      standalone/
      binary_plugin/
      single_file_plugin/
  projects/             # New projects are created here
    <project_name>/
```

### Instructions

When this command is invoked, follow these steps exactly:

1. **Parse the arguments** from `$ARGUMENTS`:
   - Extract the project type (standalone, binary_plugin, or single_file_plugin)
   - Extract the project name (strip surrounding quotes if present)
   - If the type is invalid, tell the user the valid types are: `standalone`, `binary_plugin`, `single_file_plugin`
   - If arguments are missing, show the usage format above

2. **Copy the template files**:
   - Source: `./esapi/templates/<project_type>/`
   - Destination: `./projects/<project_name>/`
   - Only copy the main source files (`.cs` and `.csproj`), do NOT copy `obj/`, `bin/`, or `.vs/` directories

3. **Rename files** in the new directory:
   - `<project_type>.cs` -> `<project_name>.cs`
   - `<project_type>.csproj` -> `<project_name>.csproj`

4. **Update file contents** - replace all occurrences of the template name with the new project name:

   In the `.csproj` file:
   - Replace `<RootNamespace>project_type</RootNamespace>` with `<RootNamespace>project_name</RootNamespace>`
   - Replace `<AssemblyName>project_type</AssemblyName>` (or `project_type.esapi` for binary_plugin) with the new name (preserving `.esapi` suffix for binary_plugin)
   - Replace `<Compile Include="project_type.cs" />` with `<Compile Include="project_name.cs" />`
   - For single_file_plugin, replace `<Compile Include="..\..\plugins\single_file_plugin.cs" />` with `<Compile Include="project_name.cs" />` (source file is now local to the project folder)
   - Generate a new random GUID for `<ProjectGuid>`

   In the `.cs` file:
   - For standalone templates: replace `namespace standalone` with `namespace project_name`
   - For binary_plugin and single_file_plugin: the namespace is `VMS.TPS` and should stay unchanged

5. **Report success**: Tell the user what was created and list the files.

---

## Subcommand: `build`

### Arguments

- `project_name`: The name of an existing project to build (strip surrounding quotes if present)
- `configuration`: Either `debug` or `release` (case-insensitive). If not provided, default to `debug`.

### Instructions

When this command is invoked, follow these steps exactly:

1. **Parse the arguments** from `$ARGUMENTS`:
   - Extract the project name (strip surrounding quotes if present)
   - Extract the configuration (`debug` or `release`). If not provided, default to `debug`.
   - If the project name is missing, show the usage format above
   - If the configuration is not `debug` or `release`, tell the user the valid options are: `debug`, `release`

2. **Verify the project exists**:
   - Check that `./projects/<project_name>/` exists
   - Check that `./projects/<project_name>/<project_name>.csproj` exists
   - If not found, tell the user the project does not exist and list available projects under `./projects/`

3. **Build the project** using MSBuild:
   - Capitalize the configuration for MSBuild: `debug` -> `Debug`, `release` -> `Release`
   - Use the Bash tool to `cd` into the project directory, then run MSBuild on the `.csproj` file with the configuration property:
     `cd "<full_path_to_project>" && "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe" <project_name>.csproj //p:Configuration=<Configuration>`
   - IMPORTANT: Use `//p:` (double slash) instead of `/p:` because Git Bash interprets single `/` as a path prefix

4. **Report the result**:
   - If the build succeeded, tell the user and show the output path (the `bin\<configuration>` directory under the project folder)
   - If the build failed, show the full error output so the user can diagnose the issue

---

## Subcommand: `run`

### Arguments

- `project_name`: The name of an existing project to run (strip surrounding quotes if present)
- `configuration`: Either `debug` or `release` (case-insensitive). If not provided, default to `debug`.

### Instructions

When this command is invoked, follow these steps exactly:

1. **Parse the arguments** from `$ARGUMENTS`:
   - Extract the project name (strip surrounding quotes if present)
   - Extract the configuration (`debug` or `release`). If not provided, default to `debug`.
   - If the project name is missing, show the usage format above
   - If the configuration is not `debug` or `release`, tell the user the valid options are: `debug`, `release`

2. **Verify the executable exists**:
   - Check that `./projects/<project_name>/bin/<configuration>/<project_name>.exe` exists
   - If not found, tell the user the executable does not exist and suggest running `/esapi build <project_name> <configuration>` first

3. **Run the executable**:
   - Use the Bash tool to execute:
     `"<full_path_to_project>/bin/<configuration>/<project_name>.exe"`

4. **Report the result**:
   - Show the program's output (stdout and stderr) to the user
   - If the program exited with a non-zero exit code, report that as well
