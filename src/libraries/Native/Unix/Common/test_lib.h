typedef struct {
    uintptr_t _field1;
    uintptr_t _field2;
} DotnetGlobalizationContext;

extern DotnetGlobalizationContext* Initialize(const void* pData);
extern uint32_t GetDateFormat(DotnetGlobalizationContext* context, uint32_t value);

extern DotnetGlobalizationContext* pIcuContext;
