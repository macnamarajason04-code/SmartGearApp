const gulp = require("gulp");
const sass = require("gulp-sass")(require("sass"));

const paths = {
  scss: "./wwwroot/css/**/*.scss",
  cssDest: "./wwwroot/css/"
};

gulp.task("sass", function () {
  return gulp
    .src(paths.scss)
    .pipe(sass({ outputStyle: "expanded" }).on("error", sass.logError))
    .pipe(gulp.dest(paths.cssDest));
});

gulp.task("watch", function () {
  gulp.watch(paths.scss, gulp.series("sass"));
});

gulp.task("default", gulp.series("sass", "watch"));
