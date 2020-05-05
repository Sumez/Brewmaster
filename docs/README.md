<img align="right" src="/logo.png" alt="Logo" style="margin: 0 0 5px 5px;" />

Download the latest official release here:
[Installer](https://github.com/Sumez/Brewmaster/releases/latest/download/BrewmasterSetup.exe)
[Zip file](https://github.com/Sumez/Brewmaster/releases/latest/download/Brewmaster.zip)

# What is Brewmaster?
### Great for beginners
Assembly programming can sound like a scary ordeal to a lot of people. Not just because the word itself gives an impression of something both complex, archaic and geeky, but also because a lot of the documentation and terminology surrounding it can feel very obtuse to anyone not already involved in it. The truth is that the 6502-style assembly language that the NES and SNES use is extremely simple, and although it is very versatile, it is also quick to learn and get started with.
For people starting out, Brewmaster provides an easy setup that will get you immediately started on creating your first NES or SNES game using assembly code. Simply import one of the pre-made templates and build it immediately. No dependencies, and no installation necessary. Just run the program and you're set.
### Perfect for seasoned programmers
The primary philosophy behind Brewmaster is that it shouldn't just do the job - but that it should feel comfortable and intuitive to work with, as one would expect of a modern Windows program.
If you are already used to creating your own assembly programs, and running an emulator to debug your code, Brewmaster should only make this process even more convenient. Simply import your existing project and build it to run it immediately in the built-in emulator, stepping through the code as you write it! Debugging a console game has never been easier.
Brewmaster is full of little tools here and there that will hopefully help speed up both the programming, data conversion, and debugging processes. And if you are missing anything, feel free to add to the issue tracker on GitHub, or contact me directly!

*- Sumez*


## Create NES and SNES games
Templates and default settings will make it easy to build both kinds of projects, and tools and emulators are available for live debugging on both platforms.
(SPC support to be added in the future)

### Intelligent configurable syntax highlighting and symbol auto-completion
Will help a lot with both regular 6502 ASM and most basic ca65 features. Some of the assembler's more advanced features might not be well supported, but feedback is very welcome!

### Easy navigation across source files
Press F12 (configurable) to immediately navigate to the definition of any recognized assembler symbol, or use the quick navigator in the top right corner to jump between symbols in the current file.

### Live error feedback
Syntax errors, unknown symbols, and similar errors that would prevent the source file from assembling will be marked instantly as you type.

### Solid built-in emulator
Game is executed using [Mesen](https://github.com/SourMesen/Mesen)'s core DLL, with the same accuracy the emulator is known for :)

### Text editor breakpoints
Execution breakpoints are based on the text editor, rather than the program counter. Place breakpoints anywhere in the source code and the program will break on the line you placed it, even after you move code around.

### Build and continue
Edit your code and create a new build while debugging, and you'll be able to continue your session with the new ROM image right where you left off.

### Built-in debug tools
All the usual stuff like memory viewer, nametable visualizer, etc. Not too expansive right now, but can potentially be expanded to include anything Mesen also has.

### Watch symbols
Hover over any address or symbol in the code to see value as both hex, decimal and word values. Add any address or symbol to the live watch to see how the value changes.

### Built-in opcode reference
No more need for a cheat sheet. Get immediate information about how each opcode works, or cross reference a list of available commands.

### Expandable data pipeline
Build your entire toolchain into the IDE by including data sources (such as tileset images, stage editor data, FamiTracker projects, etc.) and specifying how to convert them every time your project builds

### Modular workspace
Show/hide any panel, and drag/dock them around as you prefer, with the potential to make good use of a multi-monitor setup. Your workspace will be saved between sessions.


## FAQ
### My existing programming experience have been in other assemblers such as NESASM or ASM6. Does Brewmaster give me the option to use these?
Sorry. Brewmaster is designed entirely around the ca65 syntax. I might look into expanding to other assemblers eventually if there's an interest, but as it is ca65 provides a lot of debugging tools that is perfect for this design. Fortunately migrating from one assembler syntax to another is usually not very complex. The 6502 opcodes remain the same.
### Will Brewmaster be available on Linux or macOS?
Brewmaster is created using the .NET WinForms library, which should be supported by Mono, and thus portable. Right now, trying to run the program in Mono will crash it, but if anyone is up to that task, I'd be very welcoming to people willing to test the program on other platforms, or even help creating a multiplatform fork.
### Can only NES and SNES programs be created using Brewmaster?
Technically you can create anything supported by ca65 (such as Atari 2600 and C64), but the built-in emulator tools are currently only for the NES and SNES platforms (including variants and clones, such as Dendy or FDS).
If Brewmaster takes off I might look into supporting more emulators or maybe even other assemblers. But right now the focus is on improving the tools for the already supported platforms.


## Downloads
- [Latest official release](https://github.com/Sumez/Brewmaster/releases/latest)
- [See all releases (including pre-releases)](https://github.com/Sumez/Brewmaster/releases)