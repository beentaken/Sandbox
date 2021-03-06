///////////////////////////////////////////////////////////////////////
// A slight encapsulation of an OpenGL texture. This contains a method
// to read an image file into a texture, and methods to bind a texture
// to a shader for use, and unbind when done.
////////////////////////////////////////////////////////////////////////

#include "math.h"
#include <fstream>
#include <stdlib.h>

#include "texture.h"

#include <glbinding/gl/gl.h>
#include <glbinding/Binding.h>
using namespace gl;

#define STB_IMAGE_IMPLEMENTATION
#define STBI_FAILURE_USERMSG
#include "stb_image.h"
#include <unordered_map>

static std::unordered_map<std::string, Texture*> texturesLoaded;

Texture::Texture(const std::string &path) : textureId(0)
{
	const char* pathcStr = path.c_str();
	if (texturesLoaded.find(path) != texturesLoaded.end())
	{
		textureId = texturesLoaded[path]->textureId;
		return;
	}

    stbi_set_flip_vertically_on_load(true);
    int width, height, n;
    unsigned char* image = stbi_load(pathcStr, &width, &height, &n, 4);
    if (!image) {
        printf("\nRead error on file %s:\n  %s\n\n", pathcStr, stbi_failure_reason());
        exit(-1); }

    // Here we create MIPMAP and set some useful modes for the texture
    glGenTextures(1, &textureId);   // Get an integer id for thi texture from OpenGL
    glBindTexture(GL_TEXTURE_2D, textureId);
    glTexImage2D(GL_TEXTURE_2D, 0, 4, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, image);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAX_LEVEL, 100);
    glGenerateMipmap(GL_TEXTURE_2D);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, (int)GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (int)GL_LINEAR_MIPMAP_LINEAR);  
    glBindTexture(GL_TEXTURE_2D, 0);

	// insert newly loaded texture
	texturesLoaded[path] = this;

    stbi_image_free(image);
}

Texture::~Texture()
{
	
}

void Texture::Bind(const int unit, const int programId, const std::string& name)
{
    glActiveTexture((gl::GLenum)((int)GL_TEXTURE0 + unit));
    glBindTexture(GL_TEXTURE_2D, textureId);
    int loc = glGetUniformLocation(programId, name.c_str());
    glUniform1i(loc, unit);
}

void Texture::Unbind()
{  
    glBindTexture(GL_TEXTURE_2D, 0);
}
