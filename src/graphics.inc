if !def(INCLUDE_BD84485B9E064708A23575092924A03B)
def INCLUDE_BD84485B9E064708A23575092924A03B equ 1

include "gui.inc"

def windowTitlebarHeight equ 10
def windowTitlebarButtonWidth equ windowTitlebarHeight - 4
def windowBorderColor equ `33333333
def windowBorderLightColor equ `22222222
def windowFrameBackgroundColor equ `11111111
def windowFrameBackgroundLightColor equ `00000000
def windowFrameForegroundColor equ `33333333
def windowFrameInnerShadowColor equ `22222222

def windowContentBackgroundColor equ `00000000

def windowInactiveFrameForegroundColor equ `00000000
def windowInactiveFrameBackgroundColor equ `22222222
def windowInactiveFrameBackgroundLightColor equ `00000000

def buttonBackgroundColor equ `22222222
def buttonForegroundColor equ `00000000
def buttonBorderColor equ `33333333

rsreset
    def WindowDrawFlagBit_active rb 1
    def WindowDrawFlagBit_content rb 1
    assert _RS <= bitsInByte

def WindowDrawFlag_active equ 1 << WindowDrawFlagBit_active
def WindowDrawFlag_content equ 1 << WindowDrawFlagBit_content

endc
