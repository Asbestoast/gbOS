if !def(INCLUDE_4E0E3D3F1E9A4F819C1ED0F4AE2A3754)
def INCLUDE_4E0E3D3F1E9A4F819C1ED0F4AE2A3754 equ 1

include "std.inc"
include "macroUtils.inc"

macro debug_break
    if def(DEBUG)
        ld b, b
    endc
endm

;* Writes a line of text to the debug output.
;* If DEBUG is not defined, then no action is taken.
;*
;* @param strSource A label referring to the null-terminated string that will be written.
macro debug_writeLineAt
    if def(DEBUG)
        ld d, d
        jrs .debug_writeLineAt_stringData_end\@
        dw $6464
        dw $0001
        dw (\1)
        dw bank(\1)
        .debug_writeLineAt_stringData_end\@
    endc
endm

;* Writes a line of text to the debug output.
;* If DEBUG is not defined, then no action is taken.
;*
;* @param str A string constant that will be written.
macro debug_writeLine
    if def(DEBUG)
        pushs
            makeSection romx
            __string_\@: db \1, 0
        pops
        debug_writeLineAt __string_\@
    endc
endm

;* Prints an error and halts the program.
;* @param message The message to print.
;* @param [lineNumber]
;*        The current line number.
;*        This should take the form {__LINE__} (including the curly braces).
macro debug_fail
    if def(DEBUG)
        if _NARG == 1
            ; Remove quotes.
            def message\@ equs \1
            debug_writeLine "Error at {s:__FILE__}: {s:message\@}"
            purge message\@
        elif _NARG == 2
            ; Remove quotes.
            def message\@ equs \1
            def line\@ equ \2
            debug_writeLine "Error at {s:__FILE__} line {u:line\@}: {s:message\@}"
            purge line\@
            purge message\@
        else
            fail "Invalid number of arguments."
        endc
        
        debug_break
        di
        jrs @
    endc
endm

;* Prints an error and halts the program.
;* @param condition
;* @param message The message to print.
;* @param [lineNumber]
;*        The current line number.
;*        This should take the form {__LINE__} (including the curly braces).
macro debug_failIf
    if def(DEBUG)
        invertCondition def __invertedCondition\@ equs, "\1"
        jrs __invertedCondition\@, .debug_failIf_end\@
            if _NARG == 2
                debug_fail \2
            elif _NARG == 3
                debug_fail \2, \3
            else
                fail "Invalid number of arguments."
            endc
        .debug_failIf_end\@
        purge __invertedCondition\@
    endc
endm

endc
