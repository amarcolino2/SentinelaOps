#!/usr/bin/env pwsh
# Update continuity files after completing a project stage
# Usage: .\update-continuity.ps1 -CompletedStage "1.4" -NextStage "1.5" -TestCount 34 -Description "PromptVersion + InferenceExecution"

param(
    [Parameter(Mandatory=$true)]
    [string]$CompletedStage,
    
    [Parameter(Mandatory=$true)]
    [string]$NextStage,
    
    [Parameter(Mandatory=$true)]
    [int]$TestCount,
    
    [Parameter(Mandatory=$true)]
    [string]$Description,
    
    [string]$DateCompleted = (Get-Date -Format "yyyy-MM-dd"),
    [string]$RootPath = (Get-Location).Path
)

Write-Host "🔄 Atualizando arquivos de continuidade..." -ForegroundColor Cyan

# Validar arquivos existem
$files = @(
    "HOW_TO_RESUME.md",
    "NEXT_STEPS.md", 
    "IMPLEMENTATION_PROGRESS.md"
)

foreach ($file in $files) {
    if (-not (Test-Path "$RootPath\$file")) {
        Write-Host "❌ Arquivo não encontrado: $file" -ForegroundColor Red
        exit 1
    }
}

try {
    # 1. Atualizar HOW_TO_RESUME.md
    Write-Host "📝 Atualizando HOW_TO_RESUME.md..." -ForegroundColor Yellow
    $content = Get-Content "$RootPath\HOW_TO_RESUME.md" -Raw
    
    # Extrair números anteriores de estágios completados
    if ($CompletedStage -match "^(\d+)\.(\d+)$") {
        $stage_num = [int]$matches[1]
        $completed_stages = "1.1, 1.2, 1.3" # padrão inicial
        
        # Se for estágio 1.4+, adicionar aos anteriores
        if ($stage_num -gt 1 -or ([int]$matches[2] -gt 3)) {
            $completed_stages = "1.1, 1.2, 1.3, $CompletedStage"
        }
        
        $content = $content -replace 
            '(\| Etapas Completadas \|)[^|]*(\|)',
            "| Etapas Completadas | $completed_stages ✅ |"
    }
    
    $content = $content -replace 
        '(\| Próximas Etapas \|)[^|]*(\|)',
        "| Próximas Etapas | $NextStage (próximo), ... |"
    
    Set-Content "$RootPath\HOW_TO_RESUME.md" $content -Encoding UTF8
    Write-Host "✅ HOW_TO_RESUME.md atualizado" -ForegroundColor Green
    
    # 2. Atualizar NEXT_STEPS.md
    Write-Host "📝 Atualizando NEXT_STEPS.md..." -ForegroundColor Yellow
    $content = Get-Content "$RootPath\NEXT_STEPS.md" -Raw
    
    # Atualizar ÚLTIMO STATUS CONFIRMADO
    $content = $content -replace 
        '(\*\*Data\*\*:) \d{4}-\d{2}-\d{2}',
        "`$1 $DateCompleted"
    
    $content = $content -replace 
        '(\*\*Etapa Completa\*\*:) [^\n]+',
        "`$1 $CompletedStage - $Description"
    
    $content = $content -replace 
        '(\*\*Status\*\*:) ✅ BUILD OK \+ \d+',
        "`$1 ✅ BUILD OK + $TestCount"
    
    Set-Content "$RootPath\NEXT_STEPS.md" $content -Encoding UTF8
    Write-Host "✅ NEXT_STEPS.md atualizado" -ForegroundColor Green
    
    # 3. Atualizar IMPLEMENTATION_PROGRESS.md
    Write-Host "📝 Atualizando IMPLEMENTATION_PROGRESS.md..." -ForegroundColor Yellow
    $content = Get-Content "$RootPath\IMPLEMENTATION_PROGRESS.md" -Raw
    
    # Encontrar onde inserir a nova seção (antes do checklist final)
    $newSection = @"

---

### ✅ Etapa $CompletedStage - $Description

**Status**: COMPLETO

**Data de Conclusão**: $DateCompleted

**Build Status**: ✅ 18/18 projects  
**Test Status**: ✅ $TestCount tests passing  

**Próxima Etapa**: $NextStage
"@
    
    # Inserir antes da última seção de checklist
    if ($content -match "## 🔄 CHECKLIST DE RETOMADA") {
        $content = $content -replace 
            "(## 🔄 CHECKLIST DE RETOMADA)",
            "$newSection`n`n`$1"
    }
    
    Set-Content "$RootPath\IMPLEMENTATION_PROGRESS.md" $content -Encoding UTF8
    Write-Host "✅ IMPLEMENTATION_PROGRESS.md atualizado" -ForegroundColor Green
    
    Write-Host ""
    Write-Host "✨ Todos os arquivos atualizados com sucesso!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Resumo:" -ForegroundColor Cyan
    Write-Host "  📌 Etapa Completa: $CompletedStage" -ForegroundColor White
    Write-Host "  📌 Próxima Etapa: $NextStage" -ForegroundColor White
    Write-Host "  📌 Testes Passando: $TestCount" -ForegroundColor White
    Write-Host "  📌 Data: $DateCompleted" -ForegroundColor White
    Write-Host ""
    Write-Host "Próximo passo: leia NEXT_STEPS.md para ver a tarefa seguinte 🚀" -ForegroundColor Yellow
    
}
catch {
    Write-Host "❌ Erro ao atualizar arquivos:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
