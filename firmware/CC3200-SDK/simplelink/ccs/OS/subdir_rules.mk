################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Each subdirectory must supply rules for building sources it contributes
cc_pal.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/cc_pal.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/" -g --gcc --define=ccs --define=SL_FULL --define=SL_PLATFORM_MULTI_THREADED --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --gen_func_subsections=on --printf_support=full --ual --preproc_with_compile --preproc_dependency="cc_pal.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

device.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source/device.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/" -g --gcc --define=ccs --define=SL_FULL --define=SL_PLATFORM_MULTI_THREADED --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --gen_func_subsections=on --printf_support=full --ual --preproc_with_compile --preproc_dependency="device.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

driver.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source/driver.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/" -g --gcc --define=ccs --define=SL_FULL --define=SL_PLATFORM_MULTI_THREADED --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --gen_func_subsections=on --printf_support=full --ual --preproc_with_compile --preproc_dependency="driver.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

flowcont.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source/flowcont.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/" -g --gcc --define=ccs --define=SL_FULL --define=SL_PLATFORM_MULTI_THREADED --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --gen_func_subsections=on --printf_support=full --ual --preproc_with_compile --preproc_dependency="flowcont.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

fs.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source/fs.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/" -g --gcc --define=ccs --define=SL_FULL --define=SL_PLATFORM_MULTI_THREADED --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --gen_func_subsections=on --printf_support=full --ual --preproc_with_compile --preproc_dependency="fs.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

netapp.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source/netapp.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/" -g --gcc --define=ccs --define=SL_FULL --define=SL_PLATFORM_MULTI_THREADED --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --gen_func_subsections=on --printf_support=full --ual --preproc_with_compile --preproc_dependency="netapp.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

netcfg.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source/netcfg.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/" -g --gcc --define=ccs --define=SL_FULL --define=SL_PLATFORM_MULTI_THREADED --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --gen_func_subsections=on --printf_support=full --ual --preproc_with_compile --preproc_dependency="netcfg.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

nonos.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source/nonos.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/" -g --gcc --define=ccs --define=SL_FULL --define=SL_PLATFORM_MULTI_THREADED --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --gen_func_subsections=on --printf_support=full --ual --preproc_with_compile --preproc_dependency="nonos.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

socket.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source/socket.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/" -g --gcc --define=ccs --define=SL_FULL --define=SL_PLATFORM_MULTI_THREADED --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --gen_func_subsections=on --printf_support=full --ual --preproc_with_compile --preproc_dependency="socket.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

spawn.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source/spawn.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/" -g --gcc --define=ccs --define=SL_FULL --define=SL_PLATFORM_MULTI_THREADED --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --gen_func_subsections=on --printf_support=full --ual --preproc_with_compile --preproc_dependency="spawn.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

wlan.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source/wlan.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/simplelink/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/" -g --gcc --define=ccs --define=SL_FULL --define=SL_PLATFORM_MULTI_THREADED --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --gen_func_subsections=on --printf_support=full --ual --preproc_with_compile --preproc_dependency="wlan.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '


