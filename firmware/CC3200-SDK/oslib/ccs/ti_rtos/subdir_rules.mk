################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Each subdirectory must supply rules for building sources it contributes
osi_tirtos.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/osi_tirtos.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/" --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" -g --gcc --define=cc3200 --define=ccs --display_error_number --diag_warning=225 --diag_wrap=off --printf_support=full --ual --preproc_with_compile --preproc_dependency="osi_tirtos.pp" --cmd_file="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/ti_rtos/ti_rtos_config/ccs/Default/configPkg/compiler.opt" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '


