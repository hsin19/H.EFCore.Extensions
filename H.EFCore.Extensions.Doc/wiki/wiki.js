const targetPath = process.env.WIKI_FOLDER_PATH ?? '../wiki/';
const fs = require('fs');

fs.readdir(targetPath, (err, files) => {
    if (err) throw err;

    let pages = files.filter(file => file.endsWith('.md') && !file.startsWith('_'));
    pages.forEach(file => {
        fs.readFile(targetPath + file, 'utf8', function (err, data) {
            if (err) throw err;
            // replace [[text|link]] => [text](link.md)
            let newValue = data.replace(/\[\[(\w+)\|(\w+)\]\]/gm, '[$1]($2.md)');
            // replace [[link]] => [link](link.md)
            newValue = newValue.replace(/\[\[(\w+)\]\]/gm, '[$1]($1.md)');
            fs.writeFile(targetPath + file, newValue, 'utf-8', function (err) {
                if (err) throw err;
            });
        });
    });
    let toc = pages
        .sort((a, b) => {
            if (a.toUpperCase() === 'HOME.MD')
                return -1;
            else if (b.toUpperCase() === 'HOME.MD')
                return 1;
            else
                return a > b ? 1 : -1;
        })
        .map(e => '# [' + e.replace(".md", "") + '](' + e + ')')
        .join("\r\n");
    fs.writeFile(targetPath + 'toc.md', toc, 'utf-8', function (err) {
        if (err) throw err;
    });
});
