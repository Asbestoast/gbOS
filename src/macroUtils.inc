if !def(INCLUDE_ADDCB159946E422DBE4CCAE71558138B)
def INCLUDE_ADDCB159946E422DBE4CCAE71558138B equ 1

;* @param return
;* @param condition
macro invertCondition
    if strcmp(strlwr(\2), "z") == 0
        def __result\@ equs "nz"
    elif strcmp(strlwr(\2), "nz") == 0
        def __result\@ equs "z"
    elif strcmp(strlwr(\2), "c") == 0
        def __result\@ equs "nc"
    elif strcmp(strlwr(\2), "nc") == 0
        def __result\@ equs "c"
    else
        fail "Invalid argument."
    endc
    \1 "{s:__result\@}"
    purge __result\@
endm

endc
