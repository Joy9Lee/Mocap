################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Each subdirectory must supply rules for building sources it contributes
croutine.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/croutine.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/portable/CCS/ARM_CM3" -g --gcc --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --printf_support=full --ual --preproc_with_compile --preproc_dependency="croutine.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

heap_3.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/portable/MemMang/heap_3.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/portable/CCS/ARM_CM3" -g --gcc --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --printf_support=full --ual --preproc_with_compile --preproc_dependency="heap_3.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

list.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/list.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/portable/CCS/ARM_CM3" -g --gcc --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --printf_support=full --ual --preproc_with_compile --preproc_dependency="list.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

osi_freertos.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib/osi_freertos.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/portable/CCS/ARM_CM3" -g --gcc --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --printf_support=full --ual --preproc_with_compile --preproc_dependency="osi_freertos.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

port.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/portable/CCS/ARM_CM3/port.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/portable/CCS/ARM_CM3" -g --gcc --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --printf_support=full --ual --preproc_with_compile --preproc_dependency="port.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

portasm.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/portable/CCS/ARM_CM3/portasm.asm $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/portable/CCS/ARM_CM3" -g --gcc --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --printf_support=full --ual --preproc_with_compile --preproc_dependency="portasm.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

queue.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/queue.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/portable/CCS/ARM_CM3" -g --gcc --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --printf_support=full --ual --preproc_with_compile --preproc_dependency="queue.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

tasks.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/tasks.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/portable/CCS/ARM_CM3" -g --gcc --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --printf_support=full --ual --preproc_with_compile --preproc_dependency="tasks.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

timers.obj: I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/timers.c $(GEN_OPTS) $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: ARM Compiler'
	"I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/bin/armcl" -mv7M4 --code_state=16 --float_support=vfplib --abi=eabi -me -Ooff --include_path="I:/Program Files (x86)/ti/ccsv6/tools/compiler/arm_5.1.6/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/driverlib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/inc" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/oslib" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/include" --include_path="I:/TI/CC3200SDK_1.1.0/cc3200-sdk/third_party/FreeRTOS/source/portable/CCS/ARM_CM3" -g --gcc --define=cc3200 --display_error_number --diag_warning=225 --diag_wrap=off --printf_support=full --ual --preproc_with_compile --preproc_dependency="timers.pp" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

