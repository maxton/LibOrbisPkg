# LibOrbisPkg
[![Build status](https://ci.appveyor.com/api/projects/status/f0bok1ljnshd2dr0?svg=true)](https://ci.appveyor.com/project/maxton/liborbispkg/build/artifacts)

I am developing an open source library for reading and writing PS4 PKG files.
This project's goal is to eliminate the need to use proprietary SDK tools.
Without a proper open PKG tool, the PS4 homebrew scene cannot flourish. 

All code in this repository is licensed under the GNU LGPL version 3, which can be found in LICENSE.txt.

## Download
The latest builds are available to download at [AppVeyor](https://ci.appveyor.com/project/maxton/liborbispkg/build/artifacts).

## Usage

### PkgEditor
PkgEditor is a GUI tool with which you can edit GP4 projects, create and edit SFO files, and build PKG and PFS archives.
The tool also supports opening PKGs directly. You can see the header, entries, and if the package is a fake PKG or
you enter a passcode, you can browse files as well.

#### [Screenshots](https://imgur.com/a/n0cP5Ox)

![PKG Info](https://i.imgur.com/H8xJRvj.png)

![SFO Screenshot](https://i.imgur.com/6BBdxim.png)

![GP4 Screenshot](https://i.imgur.com/cjEzB6T.png)

![PKG Files](https://i.imgur.com/hT1QjcM.png)

![PKG Digest Check](https://i.imgur.com/VoHuGRF.png)

### PkgTool
PkgTool is a command line tool for common PKG/PFS/SFO tasks. Integrate it into your build scripts!

```
Usage: PkgTool.exe <verb> [options ...]

Verbs:
  pfs_buildinner <input_project.gp4> <output_pfs.dat>
    Builds an inner PFS image from the given GP4 project.

  pfs_buildouter [--encrypt] <input_project.gp4> <output_pfs.dat>
    Builds an outer PFS image, optionally encrypted, from the given GP4 project.

  pfs_extract [--verbose] <input.dat> <output_directory>
    Extracts all the files from a PFS image to the given output directory. Use the verbose flag to print filenames as they are extracted.

  pkg_build <input_project.gp4> <output_directory>
    Builds a fake PKG from the given GP4 project in the given output directory.

  pkg_extract [--verbose] [--passcode <...>] <input.pkg> <output_directory>
    Extracts all the files from a PKG to the given output directory. Use the verbose flag to print filenames as they are extracted.

  pkg_extractentry [--passcode <...>] <input.pkg> <entry_id> <output.bin>
    Extracts the selected entry from the given PKG file.

  pkg_extractinnerpfs [--passcode <...>] <input.pkg> <output_pfs.dat>
    Extracts the inner PFS image from a PKG file.

  pkg_extractouterpfs [--encrypted] [--passcode <...>] <input.pkg> <pfs_image.dat>
    Extracts and decrypts the outer PFS image from a PKG file. Use the --encrypted flag to leave the image encrypted.

  pkg_listentries <input.pkg>
    Lists the entries in a PKG file.

  pkg_makegp4 [--passcode <...>] <input.pkg> <output_dir>
    Extracts all content from the PKG and creates a GP4 project in the output directory

  pkg_validate [--verbose] <input.pkg>
    Checks the hashes and signatures of a PKG.

  sfo_deleteentry <param.sfo> <entry_name>
    Deletes the named entry from the SFO file.

  sfo_listentries <param.sfo>
    Lists the entries in an SFO file.

  sfo_new <param.sfo>
    Creates a new empty SFO file at the given path.

  sfo_setentry [--value <...>] [--type <...>] [--maxsize <...>] [--name <...>] <param.sfo> <entry_name>
    Creates or modifies the named entry in the given SFO file.
 ```

## Thanks
Everyone who helped, either directly or indirectly, but especially the following:

- flatz
- idc
