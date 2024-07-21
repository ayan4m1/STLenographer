# Stleganographer

Watermark STL files using steganography.

This program hides watermark information within the STL files geometry. It is capable of hiding the fact by applying AES encryption to the data (i.e. without a password/key the information can't be retrieved).

## Installation

> dotnet tool install --global steganographer.console

## Usage

> steganographer -h

> steganographer encode input.stl output.stl -i binary -o ascii -k password -p secret

> steganographer decode input.stl -i binary -k password
