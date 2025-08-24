import os
import time
from math import floor
from PIL import Image
import keyboard

# Register mark positions
# [0-1] values across the X and Y axis of where we should find a register mark (solid black color)
REGISTER_MARK_X = 0.03
REGISTER_MARK_Y = 0.02
# Minimum RGB value to be considered the register mark
MINIMUM_REGISTER_VALUE = 240

WAIT_TIME_BETWEEN_DIRECTORY_CHECKS = 5

SCANNED_DOCUMENTS_DIRECTORY = "ScannedDocs"

def get_pixel_color(image_path, x_ratio, y_ratio):
    """
    Gets the RGB or RGBA color of a pixel at specified coordinates in an image.

    Args:
        image_path (str): The path to the image file (e.g., 'my_image.png').
        x_ratio (float): A normalized value representing percent from left to right across the page.
        y_ratio (float): A normalized value representing percent from top to bottom across the page.

    Returns:
        tuple: A tuple representing the pixel's color (e.g., (R, G, B) or (R, G, B, A)).
               Returns None if the image cannot be opened or coordinates are out of bounds.
    """
    try:
        img = Image.open(image_path)
        # Ensure the image is in a standard color mode if you need consistent output
        # For example, convert to RGB if you don't need alpha channel:
        # img = img.convert("RGB")

        x = int( x_ratio * img.size[0] )
        y = int( y_ratio * img.size[1] )

        pixel_color = img.getpixel((x, y))
        img.close()  # Close the image file

        print(f"The color of the pixel at ({x}, {y}) is: {pixel_color}")

        return pixel_color
    except FileNotFoundError:
        print(f"Error: Image file not found at {image_path}")
        return None
    except Exception as e:
        print(f"An unexpected error occurred: {e}")
        return None

def draw_register_pixel(image_path, x_ratio, y_ratio):
    """
    Draws a reticle over the pixel at specified coordinates in an image.

    Args:
        image_path (str): The path to the image file (e.g., 'my_image.png').
        x_ratio (float): A normalized value representing percent from left to right across the page.
        y_ratio (float): A normalized value representing percent from top to bottom across the page.

    Returns:
        tuple: A tuple representing the pixel's color (e.g., (R, G, B) or (R, G, B, A)).
               Returns None if the image cannot be opened or coordinates are out of bounds.
    """
    try:
        img = Image.open(image_path)
        # Ensure the image is in a standard color mode if you need consistent output
        # For example, convert to RGB if you don't need alpha channel:
        img = img.convert("RGB")

        x = int(x_ratio * img.size[0])
        y = int(y_ratio * img.size[1])

        pixels = img.getdata()
        pixel_color = img.getpixel((x, y))

        new_pixels = []
        index = 0

        for pixel in pixels:
            r, g, b = pixel
            if index % img.size[0] == x or floor(index / img.size[1]) == y:
                #print(f"Found pixel {index} at ({x}, {y}), its color: {pixel_color}, its data is: {(r,g,b)}")
                new_pixels.append( (255, 0, 0) )
            else:
                new_pixels.append( (r, g, b) )
            index = index + 1
        img.putdata(new_pixels)

        # split directory
        split_path = image_path.split("/")
        name = split_path.pop(-1)
        name = "Altered_" + name
        path = "/".join(split_path)
        path = path + "/" + name

        print(f"Saving input image with register identified at ({path})")

        img.save(name)
        img.show(name)
        return pixel_color
    except FileNotFoundError:
        print(f"Error: Image file not found at {image_path}")
        return None
    except Exception as e:
        print(f"An unexpected error occurred: {e}")
        return None

def get_files_in_directory(directory_path):
    """
    Returns a set of filenames in the given directory.

    Args:
        directory_path (str): The path to the directory holding the files.

    Returns:
        set: A set of the files in that directory.
    """
    return set(os.listdir(directory_path))

def detect_new_files(directory_path, previous_files):
    """
    Detects and returns new files in the directory.
    Returns a set of filenames in the given directory.

    Args:
        directory_path (str): The path to the directory holding the files.
        previous_files (set): A set of previously detected files.

    Returns:
        set: A set of the files in the directory that weren't in the previously detected files set.
    """
    current_files = get_files_in_directory(directory_path)
    new_files = current_files - previous_files

    if new_files is not None:
        print(f"Detected {len(new_files)} new files in {directory_path}.")
        if len(new_files) != 0:
            print(f"New files detected in {directory_path}: {new_files}")
    else:
        print(f"No new files detected in {directory_path}.")

    return new_files

# testing docs
test_upwards_image_file = 'Test_Documents/Test_RightOrientation.png'
test_downward_image_file = 'Test_Documents/Test_FlippedOrientation.png'

# make initial list of detected files
detected_files = get_files_in_directory(SCANNED_DOCUMENTS_DIRECTORY)

should_run = True
while (should_run):
    time.sleep(WAIT_TIME_BETWEEN_DIRECTORY_CHECKS)
    # if there are any new files...
    if (new_files := detect_new_files(SCANNED_DOCUMENTS_DIRECTORY, detected_files)) is not None and len(new_files) > 0:
        # Add new files to the detected files list
        if detected_files is None or len(detected_files) == 0:
            detected_files = new_files
        else:
            detected_files.update(new_files)

        # figure out the orientation using the register marks
        for image_file in new_files :
            image_file_path = SCANNED_DOCUMENTS_DIRECTORY + "/" + image_file
            color = get_pixel_color(image_file_path,REGISTER_MARK_X,REGISTER_MARK_Y)
            if color is not None and color[0] <= MINIMUM_REGISTER_VALUE and color[1] <= MINIMUM_REGISTER_VALUE and color[2] <= MINIMUM_REGISTER_VALUE:
                print(f"{image_file_path} is oriented upwards")
            else :
                color = get_pixel_color(image_file_path,1 - REGISTER_MARK_X,1 - REGISTER_MARK_Y)
                if color is not None and color[0] <= MINIMUM_REGISTER_VALUE and color[1] <= MINIMUM_REGISTER_VALUE and color[2] <= MINIMUM_REGISTER_VALUE:
                    print(f"{image_file_path}  is oriented downwards")
