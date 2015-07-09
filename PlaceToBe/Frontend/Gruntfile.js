module.exports = function (grunt) {


  // Project configuration.
  grunt.initConfig({
    //read in our package.json
    pkg: grunt.file.readJSON('package.json'),
    //bring all our html files together to a javascript
    /*html2js: {
      options: {
        // custom options, see below
      },
      main: {
        src: ['yourhtml.html', 'another.html'],
        dest: 'templates.js'
      }
    },*/
    ngAnnotate: {
      options: {
        // Task-specific options go here.
      },
      dist: {
        files: {
          "dist/placeToBe.js":'app/src/**/*.js'
        }
      }
    },

    /*concat: {
      options: {
        // define a string to put between each file in the concatenated output
        separator: ';'
      },
      dist: {
        // the files to concatenate
        src: ['app/src/!**!/!*.js'],
        // the location of the resulting JS file
        dest: 'dist/<%= pkg.name %>.js'
      }
    },*/
    uglify: {
      options: {
        // the banner is inserted at the top of the output
        banner: '/*! <%= pkg.name %> <%= grunt.template.today("dd-mm-yyyy") %> */\n'
      },
      dist: {
        files: {
          'dist/scripts.min.js': ['dist/placeToBe.js']
        }
      }
    },
    useminPrepare: {
      options: {
        dest: 'dist'
      },
      html: 'app/index.html'
    },
    usemin: {
      options: {
        dirs: ['dist']
      },
      html: ['dist/{,*/}*.html'],
      css: ['dist/styles/{,*/}*.css']
    },

    dgeni: {
      options: {
        // Base directory of the JavaScript file to be read
        basePath: 'app/'
      },
      // JavaScript file to be read
      src: ['src/**/*.js'],
      // Directory to output the document
      dest: 'doc/'
    }

  });

  grunt.loadNpmTasks('grunt-contrib-uglify');
  grunt.loadNpmTasks('grunt-contrib-concat');
  grunt.loadNpmTasks('grunt-html2js');
  grunt.loadNpmTasks('grunt-ng-annotate');
  grunt.loadNpmTasks('grunt-dgeni');


  //grunt.registerTask('build', ['html2js']);
  //grunt.registerTask('release', ['html2js']);
  //grunt.registerTask('default', ['concat', 'uglify']);
  grunt.registerTask('jsSmall', ['ngAnnotate', 'uglify']);
};
