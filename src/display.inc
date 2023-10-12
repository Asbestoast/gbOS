if !def(INCLUDE_EA8740057F5C42148414DAEED877EAD2)
def INCLUDE_EA8740057F5C42148414DAEED877EAD2 equ 1

include "hardware.inc"

def display_vramFramebufferTileIndex equ $10
def display_vramFramebufferAddress equ _VRAM + display_vramFramebufferTileIndex * sizeof_TILE_PATTERN

endc
