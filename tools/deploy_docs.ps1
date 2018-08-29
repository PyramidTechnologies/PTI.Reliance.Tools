git config --global credential.helper store
Add-Content "$HOME\.git-credentials" "https://$($env:access_token):x-oauth-basic@github.com`n"
config --global user.name "Bert Bot"
git config --global user.email "bertbot@pyramidacceptors.com"
git clone https://github.com/PyramidTechnologies/PTI.Reliance.Tools.git -b gh-pages _site -q
docfx docsource\docfx.json
CD _site
git add -A
git diff-index --quiet HEAD
if (0 -ne $LASTEXITCODE) { 
	git commit -m "CI Updates" -q -and git push origin gh-pages -q
}