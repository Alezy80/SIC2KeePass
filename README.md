# SIC2KeePass

This plugin allows to transfer **[SafeInCloud](https://www.safe-in-cloud.com/)** databases directly or via exported XML file into **[KeePass 2.xx](http://keepass.info/)** password manager. At the moment of writing this it supports all kinds of contents from **SafeInCloud** including, but not limited, embedded files and pictures. It also supports transfer of TOTP fields compatible with **[KeeOtp](https://bitbucket.org/devinmartin/keeotp/wiki/Home)** plugin.

## Plugin Installation and Uninstallation

- Download last plugin version from the [Releases](https://github.com/Alezy80/SIC2KeePass/releases) page, file named **SafeInCloudImp.plgx**.
- Copy the plugin file into the **KeePass** directory (where the KeePass.exe is) or a subdirectory of it.
- Restart **KeePass** in order to load the new plugin.

In other words, to "install" a plugin you simply need to copy it somewhere into the **KeePass** directory. If you have plugin loading errors in Linux you may need to install the **Mono** runtime, e.g in Ubuntu based distros please install package `mono-complete` or see [instructions](https://www.mono-project.com/download/stable/#download-lin) for your distro.

To "uninstall" a plugin, delete the plugin files.

## Transfer passwords from SafeInCloud to KeePass

### From encrypted database directly
This is preferable method of data transferring.
- Open **KeePass** then create new or open existing password database.
- Select `File` → `Import...`, then in dialog box select `SafeInCloud` under category `Passwords managers`.
- In edit box `Files to be imported` open **SafeInCloud** database, which normally located in `%LOCALAPPDATA%\SafeInCloud\SafeInCloud.db` file and press OK. Enter SafeInCloud database password.

### Via XML file
This method requires that you can run **SafeInCloud** password manager, so it is not working in **Linux**. And XML file contains your passwords in unencrypted form, so you must use this method only if previous does not working.
- In desktop version of **SafeInCloud** password manager select `File` → `Export` → `As XML` and choose where to store exported file.
- Open **KeePass** then create new or open existing password database.
- Select `File` → `Import...`, then in dialog box select `SafeInCloud` under category `Passwords managers`.
- In edit box `Files to be imported` open previously exported XML file and press OK.
- Remove XML file, because it contains all sensitive data in unencrypted form.

## License

SIC2KeePass contains sources from http://dotnetzip.codeplex.com/ library which has Zlib License.

SIC2KeePass itself is MIT-licensed.

```
The MIT License (MIT)

Copyright (c) 2016-2018, Alex Zavadsky (Alezy)
 
 Permission is hereby granted, free of charge, to any person obtaining
 a copy of this software and associated documentation files (the
 "Software"), to deal in the Software without restriction, including
 without limitation the rights to use, copy, modify, merge, publish,
 distribute, sublicense, and/or sell copies of the Software, and to
 permit persons to whom the Software is furnished to do so, subject to
 the following conditions:
 
 The above copyright notice and this permission notice shall be included
 in all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```
