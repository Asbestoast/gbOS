if !def(INCLUDE_760C2C0E27D8464D8FFDB54DDCC5E9AA)
def INCLUDE_760C2C0E27D8464D8FFDB54DDCC5E9AA equ 1

rsreset
    def LoadInfo_size rb sizeof_UInt16
    def LoadInfo_dst rb sizeof_Ptr16
    def LoadInfo_src rb sizeof_Ptr16
    def sizeof_LoadInfo equ _RS

;* Marks a piece of code or data to be automatically loaded.
;* The destination label must have a local label named ".end".
;* The order that auto-load data is loaded is undefined.
;* @param dst
;* @param src
macro autoLoad
    assert ((\1) >= $8000 && (\1) <= $9FFF) || \ ; VRAM
           ((\1) >= $C000 && (\1) <= $DFFF) || \ ; WRAM
           ((\1) >= $FF80 && (\1) <= $FFFE), \ ; HRAM
           "dst must refer to a location in VRAM, WRAM, or HRAM."
    pushs
    assert (\2) < $8000, "src must refer to a location in ROM."
    assertBankAccessible \2, RomBank_autoLoad
    section fragment "__autoLoad", romx, bank[RomBank_autoLoad]
    dw ((\1.end) - (\1)) ; LoadInfo_size
    dw (\1) ; LoadInfo_dst
    dw (\2) ; LoadInfo_src
    pops
endm

if !def(__TINY_MODE)
;* @param bank<a>
macro system_setRomBank
    rst __system_setRomBank
endm

;* @destroy af
macro system_pushRomBank
    ldh a, [system_currentRomBank]
    push af
endm

;* @destroy af
macro system_popRomBank
    pop af
    system_setRomBank
endm
endc

endc
