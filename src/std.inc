if !def(INCLUDE_3206F719D11C45948D16ECFA46622AEA)
def INCLUDE_3206F719D11C45948D16ECFA46622AEA equ 1

;* Defines an anonymous section.
;* Uses the same arguments as the "section" command.
macro makeSection
    def file\@ equs __FILE__ ; Remove quotes
    section "__anonymousSection_{s:file\@}\@", \#
    purge file\@
endm

;* Marks the beginning of an anonymous load section.
;* Uses the same arguments as the "load" command.
macro beginLoadSection
    def file\@ equs __FILE__ ; Remove quotes
    load "__anonymousSection_{s:file\@}\@", \#
    purge file\@
endm

;* Marks the end of an anonymous load section.
;* A label called '.end' will be created inside the load section.
macro endLoadSection
    .end
    endl
endm

;* Asserts that the given target's bank is guaranteed to be
;* accessible given the specified bank.
;* @param target The target to check against.
;* @param bank The bank to check against.
macro __assertBankAccessibleInternal
    if !def(__TINY_MODE)
        assert (\1) >= $8000 || (\2) == bank(\1) || bank(\1) == 0, \
               "The bank of the target is not guaranteed to be accessible."
    endc
endm

;* Asserts that the given target's bank is guaranteed to be
;* accessible given the specified bank.
;* @param target The target to check against.
;* @param [bank] The bank to check against. If no bank is specified, then the bank
;*   of the insertion point is used instead.
macro assertBankAccessible
    if _NARG == 1
        __assertBankAccessibleInternal \1, bank(@)
    else
        __assertBankAccessibleInternal \1, \2
    endc
endm

macro __assertJumpTargetBankAccessible
    if _NARG == 1
        assertBankAccessible \1
    elif _NARG == 2
        assertBankAccessible \2
    else
        fail "Incorrect number of arguments."
    endc
endm

;* inserts a 'call' instruction with an assertBankAccessible check.
macro calls
    __assertJumpTargetBankAccessible \#
    call \#
endm

;* inserts a 'jp' instruction with an assertBankAccessible check.
macro jps
    __assertJumpTargetBankAccessible \#
    jp \#
endm

;* inserts a 'jr' instruction with an assertBankAccessible check.
macro jrs
    __assertJumpTargetBankAccessible \#
    jr \#
endm

;* Asserts that the given label exists at the current PC position and bank.
;* Useful for asserting that a fall-through will occur to the given label.
;* @param label The label to fall through to.
macro fallThrough
    assert @ == (\1)
    if !def(__TINY_MODE)
        assert bank(@) == bank(\1)
    endc
endm

;* Aligns _RS by the given number of bits.
;* @param alignment
macro rsalign
    rsset (_RS & ~((1 << (\1)) - 1)) + (1 << (\1))
endm

def sizeof_Int8 equ 1
def Int8_minValue equ -$80
def Int8_maxValue equ $7F
def sizeof_UInt8 equ 1
def UInt8_minValue equ 0
def UInt8_maxValue equ $FF

def sizeof_Int16 equ 2
def Int16_minValue equ -$8000
def Int16_maxValue equ $7FFF
def sizeof_UInt16 equ 2
def UInt16_minValue equ 0
def UInt16_maxValue equ $FFFF

def sizeof_Int32 equ 4
def Int32_minValue equ -$80000000
def Int32_maxValue equ $7FFFFFFF
def sizeof_UInt32 equ 4
def UInt32_minValue equ 0
def UInt32_maxValue equ $FFFFFFFF

def sizeof_Ptr16 equ 2

def Null equ 0

def True equ 1
def False equ 0

def bitsInByte equ 8

;* @param destination
macro __assertJumpIsLongJump
    def sizeof_jrInstruction\@ equ 2
    def sizeof_jpInstruction\@ equ 3
    assert (\1) < (@ + sizeof_jrInstruction\@) + Int8_minValue || \
           (\1) > (@ + sizeof_jrInstruction\@) + Int8_maxValue + (sizeof_jpInstruction\@ - sizeof_jrInstruction\@), \
           "Jump destination is too close. Use 'jrs' instead."
    purge sizeof_jrInstruction\@
    purge sizeof_jpInstruction\@
endm

;* Inserts a 'jp' instruction and asserts that the destination
;* address is too far away to use a 'jr' instruction.
;* @param [flag]
;* @param destination
macro jmp
    if _NARG == 1
        __assertJumpIsLongJump \1
    elif _NARG == 2
        __assertJumpIsLongJump \2
    else
        fail "Invalid number of arguments."
    endc
    jp \#
endm

;* Inserts a 'jp' instruction and asserts that the destination
;* address is too far away to use a 'jr' instruction.
;* @param [flag]
;* @param destination
macro jmps
    if _NARG == 1
        __assertJumpTargetBankAccessible \1
    elif _NARG == 2
        __assertJumpTargetBankAccessible \2
    else
        fail "Invalid number of arguments."
    endc
    jmp \#
endm

;* Inserts a 'halt' instruction followed by a 'nop' instruction.
;* This works around a bug involving the halt instruction while
;* interrupts are disabled.
macro halts
    halt
    nop
endm

;* Loads two 8-bit values into a register pair.
;* @param rp The register pair to load into
;* @param h The high byte to load
;* @param l The low byte to load
macro ld2
    ld \1, (low(\2) << 8) | low(\3)
endm

;* @param (startIndex)
macro beginTable
    if _NARG == 0
        rsreset
    elif _NARG == 1
        rsset (\1)
    else
        fail "Invalid number of arguments."
    endc
endm

macro makeTableItem
    assert _RS == (\1)
    rsset _RS + 1
    shift
    \#
endm

macro endTable
    assert _RS == (\1)
endm

include "system.inc"
include "hardware2.inc"
include "debug.inc"

endc
