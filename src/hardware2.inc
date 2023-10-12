if !def(INCLUDE_0C24671D99C24800919B0B0C66966CF9)
def INCLUDE_0C24671D99C24800919B0B0C66966CF9 equ 1

include "hardware.inc"

def LCDCB_ENABLE            equ 7
def LCDCB_WINMAP_SELECT     equ 6
def LCDCB_WIN_ENABLE        equ 5
def LCDCB_MAPPATTERN_SELECT equ 4
def LCDCB_BGMAP_SELECT      equ 3
def LCDCB_OBJ_SIZE          equ 2
def LCDCB_OBJ_ENABLE        equ 1
def LCDCB_BG_ENABLE         equ 0

def LCDCF_MAPPATTERN_SELECT equ 1 << LCDCB_MAPPATTERN_SELECT

def STATF_MODE equ %00000011

def IFF_HILO   equ %00010000 ; Transition from High to Low of Pin number P10-P13
def IFF_SERIAL equ %00001000 ; Serial I/O transfer end
def IFF_TIMER  equ %00000100 ; Timer Overflow
def IFF_STAT   equ %00000010 ; STAT
def IFF_VBLANK equ %00000001 ; V-Blank

def IFB_HILO   equ 4
def IFB_SERIAL equ 3
def IFB_TIMER  equ 2
def IFB_STAT   equ 1
def IFB_VBLANK equ 0

def KEY1B_DBLSPEED equ 7
def KEY1B_PREPARE  equ 0

def OAMB_PALNUM equ 0

def _SRAM_END        equ _SRAM + $2000
def _VRAM_END        equ _VRAM + $2000
def _RAM_END         equ _RAM + $2000
def _OAMRAM_END      equ _OAMRAM + $A0
def _HRAM_END        equ _HRAM + $7F
def _AUD3WAVERAM_END equ _AUD3WAVERAM + $10

;* The size of a single tile pattern, in bytes.
def sizeof_TILE_PATTERN     equ 16
def TILE_PATTERN_WIDTH      equ 8
def TILE_PATTERN_HEIGHT     equ 8

def COLOR_WHITE         equ 0
def COLOR_LIGHT_GRAY    equ 1
def COLOR_DARK_GRAY     equ 2
def COLOR_BLACK         equ 3

; Opcode defines
def OPCODE_JP       equ $C3
def OPCODE_JR       equ $18
def OPCODE_RETI     equ $D9
def OPCODE_PUSH_HL  equ $E5
def OPCODE_RST_38   equ $FF

; RST address defines
def RST_0   equ $00
def RST_8   equ $08
def RST_10  equ $10
def RST_18  equ $18
def RST_20  equ $20
def RST_28  equ $28
def RST_30  equ $30
def RST_38  equ $38

; Interrupt vector address defines
def INTV_VBLANK   equ $40
def INTV_STAT     equ $48
def INTV_TIMER    equ $50
def INTV_SERIAL   equ $58
def INTV_HILO     equ $60

; Header-related defines

rsreset
    def CartridgeHeader_entryPoint rb 4
    def CartridgeHeader_nintendoLogo rb $30
    ; ... (Incomplete)
    def sizeof_CartridgeHeader equ $50

def CARTRIDGE_HEADER  equ $100
def ENTRY_POINT       equ CARTRIDGE_HEADER + CartridgeHeader_entryPoint

endc
