﻿/// <binding ProjectOpened='watch' />
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

function lessFunc()
{
   return gulp.src('Styles/*.less')
        .pipe(less())
        .pipe(gulp.dest('wwwroot/css'));
}

gulp.task("less", lessFunc);

var min = gulp.series(gulp.parallel(minjs, mincss, function (done) {
    return minhtml(done);
}));

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

function minhtml(done)
{
    var tasks = getBundles(regex.html).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { base: "." })
            .pipe(concat(bundle.outputFileName))
            .pipe(htmlmin({ collapseWhitespace: true, minifyCSS: true, minifyJS: true }))
            .pipe(gulp.dest("."));
    });


    if (tasks && tasks.length > 0)
    {
        return merge(tasks);
    }
    else
    {
        return done();
    }
}

gulp.task("min:html", function (done) {
    return minhtml(done);
});

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
    gulp.watch('Styles/*.less', less);
    gulp.watch('Scripts/*.js', scripts);

    getBundles(regex.js).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, minjs);
    });

    getBundles(regex.css).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, mincss);
    });

    getBundles(regex.html).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, minhtml);
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
    "oidc-client": {
        "dist/*": ""
    },
    "jquery": {
        "dist/*": ""
    },
    "jquery-ajax-unobtrusive": {
        "jquery.unobtrusive-ajax.js": "",
        "jquery.unobtrusive-ajax.min.js": ""
    },
    "jquery-validation": {
        "dist/**/*": ""
    },
    "jquery-validation-unobtrusive": {
        "dist/jquery.validate.unobtrusive.js": ""
    },
    "angular": {
        "angular.js": ""
    },
    "angular-ui-bootstrap": {
        "dist/ui-bootstrap-tpls.js": ""
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
    },
    "@aspnet/signalr": {
        "dist/browser/*": ""
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