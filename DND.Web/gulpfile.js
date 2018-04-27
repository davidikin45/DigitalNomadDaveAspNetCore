/// <binding ProjectOpened='watch' />
"use strict";

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    htmlmin = require("gulp-htmlmin"),
    uglify = require("gulp-uglify"),
    merge = require("merge-stream"),
    del = require("del"),
    less = require("gulp-less"),
    bundleconfig = require("./bundleconfig.json");

var angularPaths = {
    scripts: ['Scripts_Compile/ClientAppDist/**/*'],
};

var paths = {
    scripts: ['Scripts/**/*.js', 'Scripts/**/*.cshtml','Scripts_Compile/Scripts/**/*.js', 'Scripts/**/*.ts', 'Scripts_Compile/Scripts/**/*.map'],
};

var regex = {
    less: /\.less$/,
    css: /\.css$/,
    html: /\.(html|htm)$/,
    js: /\.js$/
};

function less()
{
    return gulp.src('Styles/*.less')
        .pipe(less())
        .pipe(gulp.dest('wwwroot/css'));
}

gulp.task("less", less);

var min = gulp.series(gulp.parallel(minjs, mincss, minhtml));

gulp.task("min", min);

function minjs()
{
    var tasks = getBundles(regex.js).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(uglify())
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
}

gulp.task("min:js", minjs);

function mincss()
{
    var tasks = getBundles(regex.css).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(cssmin())
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
}

gulp.task("min:css", mincss);

function minhtml()
{
    var tasks = getBundles(regex.html).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(htmlmin({ collapseWhitespace: true, minifyCSS: true, minifyJS: true }))
            .pipe(gulp.dest("."));
    });
    return merge(tasks);
}

gulp.task("min:html", minhtml);

function clean()
{
    var files = bundleconfig.map(function (bundle) {
        return bundle.outputFileName.toString();
    });
   return del(files);
}

gulp.task("clean", clean);

function watch()
{
    gulp.watch('Styles/*.less', ['less']);
    gulp.watch('Scripts/*.js', ['scripts']);

    getBundles(regex.js).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:js"]);
    });

    getBundles(regex.css).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:css"]);
    });

    getBundles(regex.html).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, ["min:html"]);
    });
}

gulp.task("watch", watch);

function getBundles(regexPattern) {
    return bundleconfig.filter(function (bundle) {
        return regexPattern.test(bundle.outputFileName);
    });
}

// NPM Dependency Dirs
var deps = {
    "jquery": {
        "dist/*": ""
    },
    "jquery-validation": {
        "dist/**/*": ""
    },
    "jquery-validation-unobtrusive": {
        "jquery.validate.unobtrusive.js": ""
    },
    "angular": {
        "angular.js": ""
    },
    "angular-bootstrap": {
        "ui-bootstrap-tpls.js": ""
    },
    "angular-animate": {
        "angular-animate.js": ""
    },
    "ngmap": {
        "build/scripts/ng-map.js": "",
        "build/scripts/ng-map.min.js": ""
    },
    "underscore": {
        "underscore.js": ""
    },
    "bootstrap": {
        "dist/**/*": ""
    },
    "respond.js": {
        "dest/**/*": ""
    },
    "tinymce": {
        "plugins/**/*": "plugins/",
        "skins/**/*": "skins/",
        "themes/**/*": "themes/",
        "tinymce.js": "",
        "tinymce.min.js": ""
    },
    "instafeed.js": {
        "instafeed.js": "",
        "instafeed.min.js": ""
    },
    "infinite-scroll": {
        "dist/*": ""
    },
    "font-awesome": {
        "css/*": "css/",
        "fonts/*": "fonts/",
    },
    "magnific-popup": {
        "dist/*": ""
    }
};


function scripts()
{
    gulp.src(paths.scripts).pipe(gulp.dest('wwwroot/js'));
    //gulp.src(angularPaths.scripts).pipe(gulp.dest('wwwroot/js/clientapp'));

    var streams = [];

    for (var prop in deps) {
        console.log("Prepping Scripts for: " + prop);
        for (var itemProp in deps[prop]) {
            streams.push(gulp.src("node_modules/" + prop + "/" + itemProp)
                .pipe(gulp.dest("wwwroot/vendor/" + prop + "/" + deps[prop][itemProp])));
        }
    }

    return merge(streams);
}

gulp.task("scripts", scripts);