import random

def write_random_ints_to_file(file_path, num_rows):
    with open(file_path, 'w') as file:
        for _ in range(num_rows):
            random_int = random.randint(0, 3)
            file.write(f':{random_int}:\n')

# Specify the file path and the number of rows
file_path = 'Assets/Resources/song0.txt'
num_rows = 100  # Change this to the desired number of rows

# Call the function to write random integers to the file
write_random_ints_to_file(file_path, num_rows)