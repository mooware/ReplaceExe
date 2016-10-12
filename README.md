# ReplaceExe

Windows tool allowing replacement of "notepad.exe" (or any executable) with other commands.

## Motivation

ReplaceExe is my approach for replacing the atrocious "notepad.exe", which often pops up as a default text viewer and editor.

It uses a Windows feature called `Image File Execution Options`, which allows configuration of various debug options per "image name" (e.g. "notepad.exe"). One of this options is a per-image debugger; when a new process with the configured image name is started, the configured "debugger" will be started instead, with the image name and any process arguments passed to it.

ReplaceExe can be installed as such a "debugger", and will forward the invokation to the specified command.

## Usage

**To install a replacement (requires permissions to modify the Windows Registry):**

    ReplaceExe -i <image name> <replacement command and args ...>

For example:

    ReplaceExe -i notepad.exe "\"c:\Program Files (x86)\Notepad++\notepad++.exe\""

Note that the replacement command must be explicitly quoted if it contains whitespace. Also note that `ReplaceExe` will register itself as the debugger, so you should not move/rename the executable after registering it.

**To uninstall a replacement (requires permissions to modify the Windows Registry):**

    ReplaceExe -u <image name>

For example:

    ReplaceExe -u notepad.exe

## License

The source code is licensed under the **MIT license**. Its full text is included in the [LICENSE](LICENSE) file.
