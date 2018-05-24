docfx metadata -f
docfx build -f

if "%~1"=="" goto end
cd ..
cd docs
docfx --serve

:end