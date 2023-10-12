if !def(INCLUDE_5C51AB9CC2B74D238E9A742BB824EBF3)
def INCLUDE_5C51AB9CC2B74D238E9A742BB824EBF3 equ 1

;* @param returnVar
;* @param color0
;* @param color1
;* @param color2
;* @param color3
macro palette_makePalette
    \1 ((\2) | ((\3) << 2) | ((\4) << 4) | ((\5) << 6))
endm

;* @param returnVar
;* @param r
;* @param g
;* @param b
macro palette_makeColor
    \1 ((\2) | ((\3) << 5) | ((\4) << 10))
endm

endc
