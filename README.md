# SIC2KeePass

This plugin allows to transfer **[SafeInCloud](https://www.safe-in-cloud.com/)** exported XML file into **[KeePass 2.xx](http://keepass.info/)** password manager. At the moment of writing this it supports all kinds of contents from **SafeInCloud** including, but not limited, embedded files and pictures.

## Plugin Installation and Uninstallation

- Download last plugin version from the [Releases](https://github.com/Alezy80/SIC2KeePass/releases) page, file named **SafeInCloudImp.plgx**.
- Copy the plugin file into the **KeePass** directory (where the KeePass.exe is) or a subdirectory of it.
- Restart **KeePass** in order to load the new plugin.

In other words, to "install" a plugin you simply need to copy it somewhere into the **KeePass** directory.

To "uninstall" a plugin, delete the plugin files.

## Transfer passwords from SafeInCloud to KeePass

- In desktop version of **SafeInCloud** password manager select `File` → `Export` → `As XML` and choose where to store exported file.
- Open **KeePass** then create new or open existing password database.
- Select `File` → `Import...`, then in dialog box select `SafeInCloud` under category `Passwords managers`.
- In edit box `Files to be imported` open previously exported XML file and press OK.
- Remove XML file, because it contains all sensitive data in unencrypted form.

## License

SIC2KeePass is BSD-licensed.

```
Copyright (c) 2016, Alex Zavadsky (Alezy)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

    (1) Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.

    (2) Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in
    the documentation and/or other materials provided with the
    distribution.  

    (3)The name of the author may not be used to
    endorse or promote products derived from this software without
    specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT,
INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING
IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE.
```
