# LibOrbisPkg
[![Build status](https://ci.appveyor.com/api/projects/status/f0bok1ljnshd2dr0?svg=true)](https://ci.appveyor.com/project/maxton/liborbispkg/build/artifacts)

I am developing an open source library for reading and writing PS4 PKG files.
This project's goal is to eliminate the need to use proprietary SDK tools.
Without a proper open PKG tool, the PS4 homebrew scene cannot flourish. 

All code in this repository is licensed under the GNU LGPL version 3, which can be found in LICENSE.txt.

## Usage

### PkgEditor
PkgEditor is a GUI tool with which you can edit GP4 projects, and build PKG and PFS archives.
Click `File -> Open GP4...`, then click `Build PKG` to create a PKG.

![Screenshot](https://i.imgur.com/BsNFQDo.png)

The tool also supports opening PKGs directly. You can see the header, entries, and if you put the EKPFS into `<filename>.pkg.ekpfs` next to your PKG file, you can browse files as well.
![PKG Screenshot](https://i.imgur.com/EItFUff.png)

### PkgTool
```
PkgTool.exe <verb> <input> <output>

Verbs:
  makepfs <input_project.gp4> <output_pfs.dat>
  makepkg <input_project.gp4> <output_directory>
 ```

## TODO

- Reverse encryption and signatures
- Implement PFS and PKG signing
- Implement EKPFS generation

## Thanks
Everyone who helped, either directly or indirectly, but especially the following:

- flatz
