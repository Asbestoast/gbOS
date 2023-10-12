if !def(INCLUDE_4372576EB99B48CAB97246D4DCF549D4)
def INCLUDE_4372576EB99B48CAB97246D4DCF549D4 equ 1

;* Waits until the VBlank interrupt handler has been executed
;* since the last call to this function.
;* @destroy f hl
macro vBlank_waitFinished
    ld hl, _vBlank_hasFinished
    .loop\@
        srl [hl] ; _vBlank_hasFinished
        jrs c, .break\@
        halt
    jrs nc, .loop\@
    .break\@
endm

endc
