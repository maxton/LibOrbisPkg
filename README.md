# LibOrbisPkg
[![Build status](https://ci.appveyor.com/api/projects/status/f0bok1ljnshd2dr0?svg=true)](https://ci.appveyor.com/project/maxton/liborbispkg/build/artifacts)

I am developing an open source library for reading and writing PS4 PKG files.
This project's goal is to eliminate the need to use proprietary SDK tools.
Without a proper open PKG tool, the PS4 homebrew scene cannot flourish. 

All code in this repository is licensed under the GNU LGPL version 3, which can be found in LICENSE.txt.

## Usage

### PkgEditor
PkgEditor is a GUI tool with which you can edit GP4 projects, create and edit SFO files, and build PKG and PFS archives.
The tool also supports opening PKGs directly. You can see the header, entries, and if the package is a fake PKG or
you enter a passcode, you can browse files as well.

#### [Screenshots](https://imgur.com/a/n0cP5Ox)

![SFO Screenshot](https://i.imgur.com/jad4qWZ.png)

![GP4 Screenshot](https://i.imgur.com/cjEzB6T.png)

![PKG Header](https://i.imgur.com/X5zRkRt.png)

![PKG Files](https://i.imgur.com/1rvFgqC.png)

![PKG Digest Check](https://i.imgur.com/pFIVRNh.png)

### PkgTool
PkgTool is a command line tool for common PKG/PFS tasks. Integrate it into your build scripts!

```
Usage: PkgTool.exe <verb> [options ...]

Verbs:
  makepfs <input_project.gp4> <output_pfs.dat>
  makeouterpfs [--encrypt] <input_project.gp4> <output_pfs.dat>
  makepkg <input_project.gp4> <output_directory>
  extractpkg [--verbose] [--passcode <...>] <input.pkg> <output_directory>
  extractpfs [--verbose] <input.dat> <output_directory>
  extractinnerpfs [--passcode <...>] <input.pkg> <output_pfs.dat>
  extractouterpfs [--encrypted] [--passcode <...>] <input.pkg> <pfs_image.dat>
  listentries <input.pkg>
  extractentry [--decrypt] [--passcode <...>] <input.pkg> <entry_id> <output.bin>
 ```

## Thanks
Everyone who helped, either directly or indirectly, but especially the following:

- flatz
- idc
