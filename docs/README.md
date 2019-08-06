# BREWMASTER

- [x] ### Intelligent configurable syntax highlighting and symbol auto-completion
Will help a lot with regular 6502 ASM and most basic CA65 features. Some of the assembler's more advanced features might not be well supported

### Live error feedback
Syntax errors, unknown symbols, and similar errors that would prevent the source file from assembling will be marked instantly as you type

### Solid built-in emulator
Game is executed using Mesen's core DLL, with the same accuracy the emulator is known for :)

### Text editor breakpoints
Execution breakpoints are based on the text editor, rather than the program counter. Place breakpoints anywhere in the source code and the program will break on the line you placed it, even after you move code around.

### Built-in debug tools
All the usual stuff like memory viewer, nametable visualizer, etc. Not too expansive right now, but can potentially be expanded to include anything Mesen also has

### Watch symbols
Hover over any address or symbol in the code to see value as both hex, decimal and word values. Add any address or symbol to the live watch to see how the value changes

### Built-in opcode reference
No more need for a cheat-sheet. Get immediate information about how each opcode works, or cross reference a list of available commands.

### Expandable data pipeline
Build your entire toolchain into the IDE by including data sources (such as tileset images, stage editor data, FamiTracker modules, etc.) and specifying how to convert them every time your project builds

### Modular workspace
Show/hide any panel, and drag/dock them around as you prefer, with the potential to make good use of a multi-monitor setup. Your workspace will be saved between sessions.

### CA65 only
Sorry. I might expand to ASM6 eventually, since it really shouldn't involve anything aside from some changes to the highlighting scheme and a new build process.
