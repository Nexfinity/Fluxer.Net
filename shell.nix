{ pkgs ? import <nixpkgs> {} }: let
  inherit (pkgs.dotnetCorePackages)
    combinePackages
    sdk_10_0
    sdk_9_0
    sdk_8_0
    ;
  inherit (pkgs)
    mkShellNoCC
    docfx
    ;
in mkShellNoCC {
  packages = [
    (combinePackages [
      sdk_10_0
      sdk_9_0
      sdk_8_0
    ])
    docfx
  ];
}